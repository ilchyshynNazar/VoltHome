import { useCallback, useEffect, useState } from 'react'
import { fetchSolarInsights } from '../api/solarApi'
import type { SolarPaybackInsight } from '../types/solar'
import { useI18n } from '../context/I18nContext'
import { monthShort } from '../i18n/strings'
import { useStations } from '../context/StationContext'
import { Button } from '../components/ui/Button'
import './GreenTariffPage.css'

export function GreenTariffPage() {
  const { t, locale } = useI18n()
  const { selectedId } = useStations()
  const [data, setData] = useState<SolarPaybackInsight | null>(null)
  const [err, setErr] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  const load = useCallback(async () => {
    if (!selectedId) {
      setData(null)
      return
    }
    setLoading(true)
    setErr(null)
    try {
      setData(await fetchSolarInsights(selectedId))
    } catch (e) {
      setErr(e instanceof Error ? e.message : 'Error')
      setData(null)
    } finally {
      setLoading(false)
    }
  }, [selectedId])

  useEffect(() => {
    void load()
  }, [load])

  if (!selectedId) {
    return (
      <div className="vh-gt">
        <p className="vh-gt__empty">{t('noStations')}</p>
      </div>
    )
  }

  const totalPercent = (data?.monthlyProfile ?? []).reduce(
    (sum, m) => sum + m.percentOfPeak,
    0
  )

  const monthlyRevenue = (data?.monthlyProfile ?? []).map((m) => ({
    ...m,

    revenue:
      totalPercent > 0
        ? (
            (data?.estimatedAnnualRevenueUah ?? 0) *
            m.percentOfPeak
          ) / totalPercent
        : 0,
  }))

  const maxBar = Math.max(
    1,
    ...monthlyRevenue.map((m) => m.revenue)
  )
  return (
    <div className="vh-gt">
      <div className="vh-gt__head">
        <h1 className="vh-gt__title">{t('greenTariff')}</h1>
        <Button variant="ghost" onClick={() => void load()} disabled={loading}>
          {t('refresh')}
        </Button>
      </div>

      {err ? <p className="vh-gt__err">{err}</p> : null}

      {data ? (
        <>
          <section className="vh-gt__cards">
            <div className="vh-gt__card">
              <span className="vh-gt__card-label">{t('dcPower')}</span>
              <strong>{data.peakDcKw.toLocaleString()}</strong>
            </div>
            <div className="vh-gt__card">
              <span className="vh-gt__card-label">{t('acPower')}</span>
              <strong>{data.peakAcKw.toLocaleString()}</strong>
            </div>
            <div className="vh-gt__card">
              <span className="vh-gt__card-label">{t('estimatedAnnualGen')}</span>
              <strong>
                {(data.estimatedAnnualKwh / 1000).toFixed(1)} {t('kwh')}
              </strong>
            </div>
            <div className="vh-gt__card">
              <span className="vh-gt__card-label">{t('assumedTariff')}</span>
              <strong>
                {data.assumedGreenTariffUahPerKwh.toLocaleString()} ₴ / {t('kwh')}
              </strong>
            </div>
          </section>
          <p className="vh-gt__disclaimer">{data.disclaimer}</p>

          <section className="vh-gt__chart-block">
            <h2 className="vh-gt__h2">{t('monthlyIrrChart')}</h2>
            <p className="vh-gt__hint">{t('coeffProfileHint')}</p>
            <div className="vh-gt__chart" role="img" aria-label="Monthly profile">
              {monthlyRevenue.map((m) => (
                <div key={m.month} className="vh-gt__bar-wrap">

                  <div className="vh-gt__bar-area">
                    <div
                      className="vh-gt__bar"
                      style={{
                        height: `${(m.revenue / maxBar) * 100}%`
                      }}
                    />
                  </div>
                  <span className="vh-gt__bar-label">
                    {monthShort(locale, m.month)}
                  </span>
                  <span className="vh-gt__bar-val">
                    {m.revenue.toLocaleString()} ₴
                  </span>
            </div>
              ))}
            </div>
          </section>
          <div className="vh-gt__income-highlight">
            <div className="vh-gt__income-label">
              {t('estimatedYearIncome')}
            </div>
            <div className="vh-gt__income-value">
              {Math.round(data.estimatedAnnualRevenueUah).toLocaleString()} ₴
            </div>
          </div>
        </>
      ) : !loading ? (
        <p className="vh-gt__empty">—</p>
      ) : null}
    </div>
  )
}
