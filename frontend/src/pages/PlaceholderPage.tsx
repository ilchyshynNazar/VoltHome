import { useCallback, useEffect, useState } from 'react'
import { useI18n } from '../context/I18nContext'
import { useStations } from '../context/StationContext'
import { fetchSolarInsights } from '../api/solarApi'
import type { SolarPaybackInsight } from '../types/solar'
import './PlaceholderPage.css'

export function PlaceholderPage() {
  const { t } = useI18n()
  const { selectedId } = useStations()

  const [data, setData] = useState<SolarPaybackInsight | null>(null)
  const [loading, setLoading] = useState(false)
  const [err, setErr] = useState<string | null>(null)

  const load = useCallback(async () => {
    if (!selectedId) {
      setData(null)
      return
    }

    setLoading(true)
    setErr(null)

    try {
      const result = await fetchSolarInsights(selectedId)
      setData(result)
    } catch (e) {
      setErr(e instanceof Error ? e.message : 'Error')
    } finally {
      setLoading(false)
    }
  }, [selectedId])

  useEffect(() => {
    void load()
  }, [load])

  if (!selectedId) {
    return (
      <div className="vh-ph">
        <p className="vh-ph__text">{t('noStations')}</p>
      </div>
    )
  }

  return (
    <div className="vh-ph">
      <h1 className="vh-ph__title">{t('payback')}</h1>

      {err ? (
        <p className="vh-ph__text">{err}</p>
      ) : null}

      {data ? (
        <div className="vh-pay">
          <div className="vh-pay__grid">

            <div className="vh-pay__card">
              <span className="vh-pay__label">
                {t('roughSystemCost')}
              </span>

              <strong>
                {Math.round(
                  data.estimatedSystemCostUah
                ).toLocaleString()} ₴
              </strong>
            </div>

            <div className="vh-pay__card">
              <span className="vh-pay__label">
                {t('estimatedYearIncome')}
              </span>

              <strong>
                {Math.round(
                  data.estimatedAnnualRevenueUah
                ).toLocaleString()} ₴
              </strong>
            </div>

            <div className="vh-pay__card vh-pay__card--accent">
              <span className="vh-pay__label">
                {t('roughPayback')}
              </span>

              <strong>
                {data.roughPaybackYears.toLocaleString()} {t('years')}
              </strong>
            </div>

          </div>
        </div>
      ) : !loading ? (
        <p className="vh-ph__text">—</p>
      ) : null}
    </div>
  )
}