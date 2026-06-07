import { useState } from 'react'
import { FaPen, FaPlus, FaTrash } from 'react-icons/fa'
import { getErrorMessage } from '../api/http'
import { deleteStation, removePanelGroup } from '../api/solarApi'
import { useI18n } from '../context/I18nContext'
import { useToast } from '../context/ToastContext'
import { useStations } from '../context/StationContext'
import { azimuthLabel } from '../i18n/strings'
import type { SolarAzimuth } from '../types/solar'
import { StationEditorModal } from '../features/stations/StationEditorModal'
import { QuickGroupModal } from '../features/stations/QuickGroupModal'
import { QuickInverterModal } from '../features/stations/QuickInverterModal'
import { Button } from '../components/ui/Button'
import { IconButton } from '../components/ui/IconButton'
import './ConfigPage.css'

export function ConfigPage() {
  const { t, locale } = useI18n()
  const toast = useToast()
  const { stations, reload } = useStations()
  const [editorOpen, setEditorOpen] = useState(false)
  const [editorMode, setEditorMode] = useState<'create' | 'edit'>('create')
  const [editId, setEditId] = useState<string | null>(null)

  const [invModal, setInvModal] = useState<{
    stationId: string
    power: number
    eff: number
  } | null>(null)

  const [groupModal, setGroupModal] = useState<{
    stationId: string
    groupId: string | null
    initial: {
      panelCount: number
      powerPerPanel: number
      tiltAngle: number
      azimuth: SolarAzimuth
    }
  } | null>(null)

  const openCreate = () => {
    setEditorMode('create')
    setEditId(null)
    setEditorOpen(true)
  }

  const openFullEdit = (id: string) => {
    setEditorMode('edit')
    setEditId(id)
    setEditorOpen(true)
  }

  const onDeleteStation = async (id: string) => {
    if (!window.confirm(t('confirmDelete'))) return
    try {
      await deleteStation(id)
      toast.success(t('stationDeleted'))
      if (editId === id) setEditorOpen(false)
      await reload()
    } catch (e) {
      toast.error(`${t('deleteFailed')} ${getErrorMessage(e)}`)
    }
  }

  const onDeleteGroup = async (stationId: string, groupId: string) => {
    if (!window.confirm(t('removeGroup') + '?')) return
    try {
      await removePanelGroup(stationId, groupId)
      toast.success(t('stationSaved'))
      await reload()
    } catch (e) {
      toast.error(getErrorMessage(e))
    }
  }

  return (
    <div className="vh-cfg">
      <div className="vh-cfg__head">
        <h1 className="vh-cfg__title">{t('configuration')}</h1>
        <Button variant="success" onClick={openCreate} icon={<FaPlus />}>
          {t('addStation')}
        </Button>
      </div>

      <ul className="vh-cfg__list">
        {stations.map((s) => (
          <li key={s.id} className="vh-cfg__station">
            <div className="vh-cfg__station-head">
              <div>
                <h2 className="vh-cfg__name">{s.name}</h2>
                <p className="vh-cfg__region">
                  {t('region')}: {s.solarRegionName || '—'}
                </p>
              </div>
              <div className="vh-cfg__icon-row">
                <IconButton
                  icon={<FaPen />}
                  label={t('fullEdit')}
                  onClick={() => openFullEdit(s.id)}
                />
                <IconButton
                  icon={<FaPlus />}
                  label={t('addGroup')}
                  variant="success"
                  onClick={() =>
                    setGroupModal({
                      stationId: s.id,
                      groupId: null,
                      initial: {
                        panelCount: 8,
                        powerPerPanel: 400,
                        tiltAngle: 35,
                        azimuth: 0,
                      },
                    })
                  }
                />
                <IconButton
                  icon={<FaTrash />}
                  label={t('deleteStation')}
                  variant="danger"
                  onClick={() => void onDeleteStation(s.id)}
                />
              </div>
            </div>

            {s.inverter ? (
              <div className="vh-cfg__block">
                <div className="vh-cfg__block-head">
                  <h3 className="vh-cfg__h3">{t('inverter')}</h3>
                  <IconButton
                    icon={<FaPen />}
                    label={t('editInverterShort')}
                    onClick={() =>
                      setInvModal({
                        stationId: s.id,
                        power: s.inverter!.power,
                        eff: s.inverter!.efficiency,
                      })
                    }
                  />
                </div>
                <table className="vh-cfg__table">
                  <tbody>
                    <tr>
                      <td>{t('inverterPower')}</td>
                      <td>{s.inverter.power} kW</td>
                    </tr>
                    <tr>
                      <td>{t('efficiency')}</td>
                      <td>{s.inverter.efficiency}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            ) : null}

            <div className="vh-cfg__block">
              <h3 className="vh-cfg__h3">{t('groupsTab')}</h3>
              <div className="vh-cfg__groups">
                {s.panelGroups.map((g, i) => (
                  <div key={g.id} className="vh-cfg__group-card">
                    <div className="vh-cfg__group-head">
                      <span className="vh-cfg__group-title">
                        {t('groupN')} {i + 1}
                      </span>
                      <div className="vh-cfg__icon-row">
                        <IconButton
                          icon={<FaPen />}
                          label={t('editGroupShort')}
                          onClick={() =>
                            setGroupModal({
                              stationId: s.id,
                              groupId: g.id,
                              initial: {
                                panelCount: g.panelCount,
                                powerPerPanel: g.powerPerPanel,
                                tiltAngle: g.tiltAngle,
                                azimuth: g.azimuth as SolarAzimuth,
                              },
                            })
                          }
                        />
                        <IconButton
                          icon={<FaTrash />}
                          label={t('removeGroup')}
                          variant="danger"
                          disabled={s.panelGroups.length <= 1}
                          onClick={() => void onDeleteGroup(s.id, g.id)}
                        />
                      </div>
                    </div>
                    <table className="vh-cfg__table">
                      <tbody>
                        <tr>
                          <td>{t('panels')}</td>
                          <td>{g.panelCount}</td>
                        </tr>
                        <tr>
                          <td>{t('powerPerPanel')}</td>
                          <td>{g.powerPerPanel} W</td>
                        </tr>
                        <tr>
                          <td>{t('tilt')}</td>
                          <td>{g.tiltAngle}°</td>
                        </tr>
                        <tr>
                          <td>{t('azimuth')}</td>
                          <td>
                            {azimuthLabel(g.azimuth, locale)} ({g.azimuth}°)
                          </td>
                        </tr>
                      </tbody>
                    </table>
                  </div>
                ))}
              </div>
            </div>
          </li>
        ))}
      </ul>

      {stations.length === 0 ? (
        <p className="vh-cfg__empty">{t('noStations')}</p>
      ) : null}

      <StationEditorModal
        open={editorOpen}
        onClose={() => setEditorOpen(false)}
        mode={editorMode}
        stationId={editId}
        onSaved={() => void reload()}
      />

      {invModal ? (
        <QuickInverterModal
          open
          onClose={() => setInvModal(null)}
          stationId={invModal.stationId}
          initialPower={invModal.power}
          initialEfficiency={invModal.eff}
          onSaved={() => void reload()}
        />
      ) : null}

      {groupModal ? (
        <QuickGroupModal
          open
          onClose={() => setGroupModal(null)}
          stationId={groupModal.stationId}
          groupId={groupModal.groupId}
          initial={groupModal.initial}
          onSaved={() => void reload()}
        />
      ) : null}
    </div>
  )
}
