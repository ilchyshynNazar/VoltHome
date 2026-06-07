import { useEffect, useState } from 'react'
import { getErrorMessage } from '../../api/http'
import { addPanelGroup, updatePanelGroup } from '../../api/solarApi'
import { Button } from '../../components/ui/Button'
import { Modal } from '../../components/ui/Modal'
import { useI18n } from '../../context/I18nContext'
import { useToast } from '../../context/ToastContext'
import { azimuthLabel } from '../../i18n/strings'
import { SOLAR_AZIMUTHS, type SolarAzimuth } from '../../types/solar'
import './QuickModal.css'

type Props = {
  open: boolean
  onClose: () => void
  stationId: string
  /** null = create */
  groupId: string | null
  initial: {
    panelCount: number
    powerPerPanel: number
    tiltAngle: number
    azimuth: SolarAzimuth
  }
  onSaved: () => void
}

export function QuickGroupModal({
  open,
  onClose,
  stationId,
  groupId,
  initial,
  onSaved,
}: Props) {
  const { t, locale } = useI18n()
  const toast = useToast()
  const [countStr, setCountStr] = useState(String(initial.panelCount))
  const [powerStr, setPowerStr] = useState(String(initial.powerPerPanel))
  const [tilt, setTilt] = useState(initial.tiltAngle)
  const [azimuth, setAzimuth] = useState<SolarAzimuth>(initial.azimuth)
  const [errors, setErrors] = useState<Record<string, string>>({})
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    if (!open) return
    setCountStr(String(initial.panelCount))
    setPowerStr(String(initial.powerPerPanel))
    setTilt(initial.tiltAngle)
    setAzimuth(initial.azimuth)
    setErrors({})
  }, [open, initial])

  const save = async () => {
    const next: Record<string, string> = {}
    const panelCount = Number.parseInt(countStr.trim(), 10)
    const powerPerPanel = Number.parseInt(
      powerStr.trim().replace(',', '.'),
      10
    )
    if (!countStr.trim() || !Number.isFinite(panelCount) || panelCount < 1)
      next.count = t('invalidNumber')
    if (
      !powerStr.trim() ||
      !Number.isFinite(powerPerPanel) ||
      powerPerPanel < 1
    )
      next.power = t('invalidNumber')
    if (tilt < 0 || tilt > 90) next.tilt = t('invalidNumber')
    setErrors(next)
    if (Object.keys(next).length) return

    const body = {
      panelCount,
      powerPerPanel,
      tiltAngle: tilt,
      azimuth,
    }
    setBusy(true)
    try {
      if (groupId) await updatePanelGroup(stationId, groupId, body)
      else await addPanelGroup(stationId, body)
      toast.success(t('stationSaved'))
      onSaved()
      onClose()
    } catch (e) {
      toast.error(getErrorMessage(e))
    } finally {
      setBusy(false)
    }
  }

  const title = groupId ? t('editGroupShort') : t('addGroup')

  return (
    <Modal
      open={open}
      onClose={onClose}
      title={title}
      wide
      footer={
        <>
          <Button variant="ghost" onClick={onClose} disabled={busy}>
            {t('cancel')}
          </Button>
          <Button variant="primary" onClick={() => void save()} disabled={busy}>
            {t('save')}
          </Button>
        </>
      }
    >
      <div className="vh-qm-fields">
        <label className="vh-st-field">
          <span>{t('panels')}</span>
          <input
            className="vh-st-input"
            inputMode="numeric"
            value={countStr}
            onChange={(e) => setCountStr(e.target.value)}
          />
          {errors.count ? (
            <span className="vh-field-err">{errors.count}</span>
          ) : null}
        </label>
        <label className="vh-st-field">
          <span>{t('powerPerPanel')}</span>
          <input
            className="vh-st-input"
            inputMode="decimal"
            value={powerStr}
            onChange={(e) => setPowerStr(e.target.value)}
          />
          {errors.power ? (
            <span className="vh-field-err">{errors.power}</span>
          ) : null}
        </label>
        <label className="vh-st-field">
          <span>
            {t('tilt')}: {tilt}°
          </span>
          <input
            className="vh-st-range"
            type="range"
            min={0}
            max={90}
            value={tilt}
            onChange={(e) => setTilt(Number(e.target.value))}
          />
        </label>
        <label className="vh-st-field">
          <span>{t('azimuth')}</span>
          <select
            className="vh-st-input"
            value={azimuth}
            onChange={(e) =>
              setAzimuth(Number(e.target.value) as SolarAzimuth)
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
    </Modal>
  )
}
