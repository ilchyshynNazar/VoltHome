import { useEffect, useRef, useState } from 'react'
import { FaTrash } from 'react-icons/fa'
import {
  addPanelGroup,
  createStation,
  fetchRegions,
  fetchStation,
  removePanelGroup,
  updateInverter,
  updatePanelGroup,
  updateStation,
} from '../../api/solarApi'
import { Button } from '../../components/ui/Button'
import { IconButton } from '../../components/ui/IconButton'
import { Modal } from '../../components/ui/Modal'
import { getErrorMessage } from '../../api/http'
import { useI18n } from '../../context/I18nContext'
import { useToast } from '../../context/ToastContext'
import { azimuthLabel } from '../../i18n/strings'
import {
  SOLAR_AZIMUTHS,
  type SolarAzimuth,
  type SolarRegionOption,
} from '../../types/solar'
import './StationEditorModal.css'

type Tab = 'basic' | 'inverter' | 'groups'

type GroupRow = {
  clientKey: string
  id?: string
  panelCountStr: string
  powerPerPanelStr: string
  tiltAngle: number
  azimuth: SolarAzimuth
}

type FieldErrors = Record<string, string>

type Props = {
  open: boolean
  onClose: () => void
  mode: 'create' | 'edit'
  stationId: string | null
  onSaved: () => void
}

function emptyGroup(): GroupRow {
  return {
    clientKey: crypto.randomUUID(),
    panelCountStr: '8',
    powerPerPanelStr: '400',
    tiltAngle: 35,
    azimuth: 0,
  }
}

function parsePositiveInt(raw: string): number | null {
  const t = raw.trim()
  if (t === '') return null
  const n = Number.parseInt(t, 10)
  if (!Number.isFinite(n) || n <= 0) return null
  return n
}

function parsePositiveFloat(raw: string): number | null {
  const t = raw.trim().replace(',', '.')
  if (t === '') return null
  const n = Number.parseFloat(t)
  if (!Number.isFinite(n) || n <= 0) return null
  return n
}

function parseEfficiency(raw: string): number | null {
  const t = raw.trim().replace(',', '.')
  if (t === '') return null
  const n = Number.parseFloat(t)
  if (!Number.isFinite(n) || n < 0.5 || n > 1) return null
  return n
}

export function StationEditorModal({
  open,
  onClose,
  mode,
  stationId,
  onSaved,
}: Props) {
  const { t, locale } = useI18n()
  const toast = useToast()
  const [tab, setTab] = useState<Tab>('basic')
  const [regions, setRegions] = useState<SolarRegionOption[]>([])
  const [name, setName] = useState('')
  const [regionId, setRegionId] = useState('')
  const [inverterPowerStr, setInverterPowerStr] = useState('10')
  const [inverterEffStr, setInverterEffStr] = useState('0.97')
  const [groups, setGroups] = useState<GroupRow[]>([emptyGroup()])
  const [busy, setBusy] = useState(false)
  const [loadError, setLoadError] = useState<string | null>(null)
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({})
  const [saveError, setSaveError] = useState<string | null>(null)

  const initialGroupIdsRef = useRef<Set<string>>(new Set())

  useEffect(() => {
    if (!open) return
    setTab('basic')
    setFieldErrors({})
    setSaveError(null)
    setLoadError(null)
    let cancelled = false

    ;(async () => {
      try {
        const regs = await fetchRegions()
        if (cancelled) return
        setRegions(regs)

        if (mode === 'create') {
          setName('')
          setRegionId(regs[0]?.id ?? '')
          setInverterPowerStr('10')
          setInverterEffStr('0.97')
          setGroups([emptyGroup()])
          initialGroupIdsRef.current = new Set()
        } else if (stationId) {
          const s = await fetchStation(stationId)
          if (cancelled) return
          setName(s.name)
          setRegionId(s.solarRegionId)
          setInverterPowerStr(
            s.inverter?.power != null ? String(s.inverter.power) : '10'
          )
          setInverterEffStr(
            s.inverter?.efficiency != null
              ? String(s.inverter.efficiency)
              : '0.97'
          )
          const rows: GroupRow[] = s.panelGroups.map((g) => ({
            clientKey: g.id,
            id: g.id,
            panelCountStr: String(g.panelCount),
            powerPerPanelStr: String(g.powerPerPanel),
            tiltAngle: g.tiltAngle,
            azimuth: g.azimuth as SolarAzimuth,
          }))
          setGroups(rows.length ? rows : [emptyGroup()])
          initialGroupIdsRef.current = new Set(s.panelGroups.map((g) => g.id))
        }
      } catch (e) {
        if (!cancelled) {
          setLoadError(e instanceof Error ? e.message : 'Error')
        }
      }
    })()

    return () => {
      cancelled = true
    }
  }, [open, mode, stationId])

  const addGroup = () => setGroups((g) => [...g, emptyGroup()])

  const removeGroupAt = (key: string) => {
    setGroups((list) => {
      if (list.length <= 1) return list
      return list.filter((x) => x.clientKey !== key)
    })
    setFieldErrors((e) => {
      const next = { ...e }
      delete next[`g-${key}-panels`]
      delete next[`g-${key}-power`]
      return next
    })
  }

  const patchGroup = (key: string, patch: Partial<GroupRow>) => {
    setGroups((list) =>
      list.map((x) => (x.clientKey === key ? { ...x, ...patch } : x))
    )
  }

  const validate = (): {
    ok: false
    errors: FieldErrors
  } | {
    ok: true
    inverterPower: number
    inverterEff: number
    groupsParsed: Array<{
      clientKey: string
      id?: string
      panelCount: number
      powerPerPanel: number
      tiltAngle: number
      azimuth: SolarAzimuth
    }>
  } => {
    const errors: FieldErrors = {}
    if (!name.trim()) errors.name = t('required')
    if (!regionId) errors.regionId = t('required')

    const invP = parsePositiveFloat(inverterPowerStr)
    if (invP == null) errors.invPower = t('invalidNumber')

    const invE = parseEfficiency(inverterEffStr)
    if (invE == null) errors.invEff = t('invalidNumber')

    const groupsParsed: Array<{
      clientKey: string
      id?: string
      panelCount: number
      powerPerPanel: number
      tiltAngle: number
      azimuth: SolarAzimuth
    }> = []

    if (!groups.length) errors.groups = t('required')

    for (const g of groups) {
      const pc = parsePositiveInt(g.panelCountStr)
      if (pc == null) errors[`g-${g.clientKey}-panels`] = t('invalidNumber')
      const pp = parsePositiveInt(g.powerPerPanelStr)
      if (pp == null) errors[`g-${g.clientKey}-power`] = t('invalidNumber')
      if (g.tiltAngle < 0 || g.tiltAngle > 90) {
        errors[`g-${g.clientKey}-tilt`] = t('invalidNumber')
      }
    }

    if (Object.keys(errors).length > 0) {
      return { ok: false, errors }
    }

    for (const g of groups) {
      groupsParsed.push({
        clientKey: g.clientKey,
        id: g.id,
        panelCount: parsePositiveInt(g.panelCountStr)!,
        powerPerPanel: parsePositiveInt(g.powerPerPanelStr)!,
        tiltAngle: g.tiltAngle,
        azimuth: g.azimuth,
      })
    }

    return {
      ok: true,
      inverterPower: invP!,
      inverterEff: invE!,
      groupsParsed,
    }
  }

  const onSave = async () => {
    const v = validate()
    if (!v.ok) {
      setFieldErrors(v.errors)
      setSaveError(null)
      return
    }
    setBusy(true)
    setFieldErrors({})
    setSaveError(null)
    try {
      const payloadGroup = (g: (typeof v.groupsParsed)[0]) => ({
        panelCount: g.panelCount,
        powerPerPanel: g.powerPerPanel,
        tiltAngle: g.tiltAngle,
        azimuth: g.azimuth,
      })

      if (mode === 'create') {
        await createStation({
          name: name.trim(),
          solarRegionId: regionId,
          inverter: {
            power: v.inverterPower,
            efficiency: v.inverterEff,
          },
          panelGroups: v.groupsParsed.map(payloadGroup),
        })
      } else if (stationId) {
        await updateStation(stationId, {
          name: name.trim(),
          solarRegionId: regionId,
        })
        await updateInverter(stationId, {
          power: v.inverterPower,
          efficiency: v.inverterEff,
        })

        const initial = initialGroupIdsRef.current
        const currentIds = new Set(
          v.groupsParsed.filter((g) => g.id).map((g) => g.id as string)
        )
        for (const gid of initial) {
          if (!currentIds.has(gid)) {
            await removePanelGroup(stationId, gid)
          }
        }
        for (const g of v.groupsParsed) {
          const p = payloadGroup(g)
          if (g.id) {
            await updatePanelGroup(stationId, g.id, p)
          } else {
            await addPanelGroup(stationId, p)
          }
        }
      }
      toast.success(t('stationSaved'))
      onSaved()
      onClose()
    } catch (e) {
      const msg = getErrorMessage(e)
      setSaveError(msg)
      toast.error(msg || t('stationSaveFailed'))
    } finally {
      setBusy(false)
    }
  }

  const title =
    mode === 'create' ? t('addStation') : `${t('editStation')} — ${name}`

  return (
    <Modal
      open={open}
      onClose={onClose}
      title={title}
      xl
      footer={
        <>
          <Button variant="ghost" onClick={onClose} disabled={busy}>
            {t('cancel')}
          </Button>
          <Button variant="primary" onClick={() => void onSave()} disabled={busy}>
            {t('save')}
          </Button>
        </>
      }
    >
      <div className="vh-st-tabs">
        <button
          type="button"
          className={`vh-st-tab ${tab === 'basic' ? 'vh-st-tab--on' : ''}`}
          onClick={() => setTab('basic')}
        >
          {t('basicTab')}
        </button>
        <button
          type="button"
          className={`vh-st-tab ${tab === 'inverter' ? 'vh-st-tab--on' : ''}`}
          onClick={() => setTab('inverter')}
        >
          {t('inverterTab')}
        </button>
        <button
          type="button"
          className={`vh-st-tab ${tab === 'groups' ? 'vh-st-tab--on' : ''}`}
          onClick={() => setTab('groups')}
        >
          {t('groupsTab')}
        </button>
      </div>

      {loadError ? <p className="vh-st-err">{loadError}</p> : null}
      {saveError ? <p className="vh-st-err">{saveError}</p> : null}
      {fieldErrors.groups ? (
        <p className="vh-st-err">{fieldErrors.groups}</p>
      ) : null}

      {tab === 'basic' ? (
        <div className="vh-st-fields">
          <label className="vh-st-field">
            <span>{t('stationName')}</span>
            <input
              className={`vh-st-input ${fieldErrors.name ? 'vh-st-input--err' : ''}`}
              value={name}
              onChange={(e) => {
                setName(e.target.value)
                if (fieldErrors.name) {
                  setFieldErrors((x) => {
                    const n = { ...x }
                    delete n.name
                    return n
                  })
                }
              }}
            />
            {fieldErrors.name ? (
              <span className="vh-st-field-err">{fieldErrors.name}</span>
            ) : null}
          </label>
          <label className="vh-st-field">
            <span>{t('region')}</span>
            <select
              className={`vh-st-input ${fieldErrors.regionId ? 'vh-st-input--err' : ''}`}
              value={regionId}
              onChange={(e) => {
                setRegionId(e.target.value)
                if (fieldErrors.regionId) {
                  setFieldErrors((x) => {
                    const n = { ...x }
                    delete n.regionId
                    return n
                  })
                }
              }}
            >
              {regions.map((r) => (
                <option key={r.id} value={r.id}>
                  {r.name}
                </option>
              ))}
            </select>
            {fieldErrors.regionId ? (
              <span className="vh-st-field-err">{fieldErrors.regionId}</span>
            ) : null}
          </label>
        </div>
      ) : null}

      {tab === 'inverter' ? (
        <div className="vh-st-fields">
          <label className="vh-st-field">
            <span>{t('inverterPower')}</span>
            <input
              className={`vh-st-input ${fieldErrors.invPower ? 'vh-st-input--err' : ''}`}
              inputMode="decimal"
              value={inverterPowerStr}
              onChange={(e) => {
                setInverterPowerStr(e.target.value)
                if (fieldErrors.invPower) {
                  setFieldErrors((x) => {
                    const n = { ...x }
                    delete n.invPower
                    return n
                  })
                }
              }}
            />
            {fieldErrors.invPower ? (
              <span className="vh-st-field-err">{fieldErrors.invPower}</span>
            ) : null}
          </label>
          <label className="vh-st-field">
            <span>{t('efficiency')}</span>
            <input
              className={`vh-st-input ${fieldErrors.invEff ? 'vh-st-input--err' : ''}`}
              inputMode="decimal"
              value={inverterEffStr}
              onChange={(e) => {
                setInverterEffStr(e.target.value)
                if (fieldErrors.invEff) {
                  setFieldErrors((x) => {
                    const n = { ...x }
                    delete n.invEff
                    return n
                  })
                }
              }}
            />
            {fieldErrors.invEff ? (
              <span className="vh-st-field-err">{fieldErrors.invEff}</span>
            ) : null}
          </label>
        </div>
      ) : null}

      {tab === 'groups' ? (
        <div className="vh-st-groups">
          {groups.map((g, idx) => (
            <div key={g.clientKey} className="vh-st-group">
              <div className="vh-st-group__head">
                <span className="vh-st-group__title">
                  {t('groupN')} {idx + 1}
                </span>
                <IconButton
                  icon={<FaTrash />}
                  label={t('removeGroup')}
                  variant="danger"
                  disabled={groups.length <= 1}
                  onClick={() => removeGroupAt(g.clientKey)}
                />
              </div>
              <label className="vh-st-field">
                <span>{t('panels')}</span>
                <input
                  className={`vh-st-input ${fieldErrors[`g-${g.clientKey}-panels`] ? 'vh-st-input--err' : ''}`}
                  inputMode="numeric"
                  value={g.panelCountStr}
                  onChange={(e) => {
                    patchGroup(g.clientKey, {
                      panelCountStr: e.target.value,
                    })
                    const k = `g-${g.clientKey}-panels`
                    if (fieldErrors[k]) {
                      setFieldErrors((x) => {
                        const n = { ...x }
                        delete n[k]
                        return n
                      })
                    }
                  }}
                />
                {fieldErrors[`g-${g.clientKey}-panels`] ? (
                  <span className="vh-st-field-err">
                    {fieldErrors[`g-${g.clientKey}-panels`]}
                  </span>
                ) : null}
              </label>
              <label className="vh-st-field">
                <span>{t('powerPerPanel')}</span>
                <input
                  className={`vh-st-input ${fieldErrors[`g-${g.clientKey}-power`] ? 'vh-st-input--err' : ''}`}
                  inputMode="numeric"
                  value={g.powerPerPanelStr}
                  onChange={(e) => {
                    patchGroup(g.clientKey, {
                      powerPerPanelStr: e.target.value,
                    })
                    const k = `g-${g.clientKey}-power`
                    if (fieldErrors[k]) {
                      setFieldErrors((x) => {
                        const n = { ...x }
                        delete n[k]
                        return n
                      })
                    }
                  }}
                />
                {fieldErrors[`g-${g.clientKey}-power`] ? (
                  <span className="vh-st-field-err">
                    {fieldErrors[`g-${g.clientKey}-power`]}
                  </span>
                ) : null}
              </label>
              <label className="vh-st-field">
                <span>
                  {t('tilt')}: {g.tiltAngle}°
                </span>
                <input
                  className="vh-st-range"
                  type="range"
                  min={0}
                  max={90}
                  value={g.tiltAngle}
                  onChange={(e) =>
                    patchGroup(g.clientKey, {
                      tiltAngle: Number(e.target.value),
                    })
                  }
                />
                {fieldErrors[`g-${g.clientKey}-tilt`] ? (
                  <span className="vh-st-field-err">
                    {fieldErrors[`g-${g.clientKey}-tilt`]}
                  </span>
                ) : null}
              </label>
              <label className="vh-st-field">
                <span>{t('azimuth')}</span>
                <select
                  className="vh-st-input"
                  value={g.azimuth}
                  onChange={(e) =>
                    patchGroup(g.clientKey, {
                      azimuth: Number(e.target.value) as SolarAzimuth,
                    })
                  }
                >
                  {SOLAR_AZIMUTHS.map((deg) => (
                    <option key={deg} value={deg}>
                      {azimuthLabel(deg, locale)} ({deg}°)
                    </option>
                  ))}
                </select>
              </label>
            </div>
          ))}
          <Button variant="success" type="button" onClick={addGroup}>
            {t('addGroup')}
          </Button>
        </div>
      ) : null}
    </Modal>
  )
}
