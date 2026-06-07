import { useEffect, useState } from 'react'
import { getErrorMessage } from '../../api/http'
import { updateInverter } from '../../api/solarApi'
import { Button } from '../../components/ui/Button'
import { Modal } from '../../components/ui/Modal'
import { useI18n } from '../../context/I18nContext'
import { useToast } from '../../context/ToastContext'
import './QuickModal.css'

type Props = {
  open: boolean
  onClose: () => void
  stationId: string
  initialPower: number
  initialEfficiency: number
  onSaved: () => void
}

export function QuickInverterModal({
  open,
  onClose,
  stationId,
  initialPower,
  initialEfficiency,
  onSaved,
}: Props) {
  const { t } = useI18n()
  const toast = useToast()
  const [powerStr, setPowerStr] = useState(String(initialPower))
  const [effStr, setEffStr] = useState(String(initialEfficiency))
  const [errors, setErrors] = useState<Record<string, string>>({})
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    if (!open) return
    setPowerStr(String(initialPower))
    setEffStr(String(initialEfficiency))
    setErrors({})
  }, [open, initialPower, initialEfficiency])

  const save = async () => {
    const next: Record<string, string> = {}
    const power = parseFloat(powerStr.replace(',', '.'))
    const eff = parseFloat(effStr.replace(',', '.'))
    if (!powerStr.trim() || Number.isNaN(power) || power <= 0)
      next.power = t('invalidNumber')
    if (!effStr.trim() || Number.isNaN(eff) || eff < 0.5 || eff > 1)
      next.eff = t('invalidNumber')
    setErrors(next)
    if (Object.keys(next).length) return

    setBusy(true)
    try {
      await updateInverter(stationId, { power, efficiency: eff })
      toast.success(t('stationSaved'))
      onSaved()
      onClose()
    } catch (e) {
      toast.error(getErrorMessage(e))
    } finally {
      setBusy(false)
    }
  }

  return (
    <Modal
      open={open}
      onClose={onClose}
      title={t('editInverterShort')}
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
          <span>{t('inverterPower')}</span>
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
          <span>{t('efficiency')}</span>
          <input
            className="vh-st-input"
            inputMode="decimal"
            value={effStr}
            onChange={(e) => setEffStr(e.target.value)}
          />
          {errors.eff ? (
            <span className="vh-field-err">{errors.eff}</span>
          ) : null}
        </label>
      </div>
    </Modal>
  )
}
