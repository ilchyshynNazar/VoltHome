import { useCallback, useEffect, useState } from 'react'
import { useI18n } from '../context/I18nContext'
import { useStations } from '../context/StationContext'
import { fetchDashboard, fetchStation } from '../api/solarApi'
import type { SolarDashboardDto, SolarStationDto } from '../types/solar'
import { azimuthLabel } from '../i18n/strings'
import { Button } from '../components/ui/Button'
import { SemiGauge } from '../components/ui/SemiGauge'
import './MonitoringPage.css'

export function MonitoringPage() {
  const { t, locale } = useI18n()
  const { selectedId } = useStations()
  const [dash, setDash] = useState<SolarDashboardDto | null>(null)
  const [station, setStation] = useState<SolarStationDto | null>(null)
  const [err, setErr] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)
  const [mounted, setMounted] = useState(false)

  const load = useCallback(async () => {
    if (!selectedId) {
      setDash(null)
      setStation(null)
      return
    }
    setLoading(true)
    setErr(null)
    try {
      const [d, s] = await Promise.all([
        fetchDashboard(selectedId),
        fetchStation(selectedId),
      ])
      setDash(d)
      setStation(s)
    } catch (e) {
      setErr(e instanceof Error ? e.message : 'Error')
    } finally {
      setLoading(false)
    }
  }, [selectedId])

  useEffect(() => {
    void load()
  }, [load])

  useEffect(() => {
    setMounted(true)
  }, [])

  useEffect(() => {
    const id = window.setInterval(() => void load(), 60_000)
    return () => window.clearInterval(id)
  }, [load])

  if (!selectedId) {
    return (
      <div className="vh-mon">
        <p className="vh-mon__empty">{t('noStations')}</p>
      </div>
    )
  }

  const hourMax =
    dash && dash.hourGaugeMax > 0 ? dash.hourGaugeMax : 1
  const dayMax = dash && dash.dayGaugeMax > 0 ? dash.dayGaugeMax : 1

  const peakDcKw = station
    ? station.panelGroups.reduce(
        (s, g) => s + (g.panelCount * g.powerPerPanel) / 1000,
        0
      )
    : 0
  const peakAcKw =
    station?.inverter != null
      ? Math.min(peakDcKw, station.inverter.power)
      : peakDcKw

    return (
    <div className={`vh-mon ${mounted ? 'vh-mon--anim' : ''}`}>
      <div className="vh-mon__head">
        <h1 className="vh-mon__h">{t('monitoring')}</h1>
        <Button variant="ghost" onClick={() => void load()} disabled={loading}>
          {t('refresh')}
        </Button>
      </div>

      {err ? <p className="vh-mon__err">{err}</p> : null}

      <div className="vh-mon__gauges">
        <SemiGauge
          value={dash?.currentHourKwh ?? 0}
          max={hourMax}
          label={t('hourProduction')}
          sublabel={`${t('peakScale')}: ≈ ${hourMax.toLocaleString(undefined, { maximumFractionDigits: 2 })} ${t('kwh')}`}
          large
        />
        <SemiGauge
          value={dash?.todayKwh ?? 0}
          max={dayMax}
          label={t('dayProduction')}
          sublabel={`${t('peakScale')}: ≈ ${dayMax.toLocaleString(undefined, { maximumFractionDigits: 1 })} ${t('kwh')}`}
          large
        />
      </div>
      {dash ? (
        <div className="vh-mon__forecast">
          <h3 className="vh-mon__h3">{t('forecast')}</h3>

          <div className="vh-mon__forecast-grid">
            <div className="vh-mon__cell">
              <span className="vh-mon__k">Next hour</span>
              <span className="vh-mon__v">
                {dash.forecastNextHourKwh.toFixed(2)} kWh
              </span>
            </div>

            <div className="vh-mon__cell">
              <span className="vh-mon__k">Remaining today (ML)</span>
              <span className="vh-mon__v">
                {dash.forecastTodayRemainingKwh.toFixed(2)} kWh
              </span>
            </div>
          </div>
        </div>
      ) : null}

      {station ? (
        <section className="vh-mon__detail">
          <h2 className="vh-mon__detail-title">{t('stationDetailTitle')}</h2>
          <div className="vh-mon__grid">
            <div className="vh-mon__cell">
              <span className="vh-mon__k">{t('stationName')}</span>
              <span className="vh-mon__v">{station.name}</span>
            </div>
            <div className="vh-mon__cell">
              <span className="vh-mon__k">{t('region')}</span>
              <span className="vh-mon__v">
                {station.solarRegionName || '—'}
              </span>
            </div>
            <div className="vh-mon__cell">
              <span className="vh-mon__k">{t('dcPower')}</span>
              <span className="vh-mon__v">
                {peakDcKw.toLocaleString(undefined, { maximumFractionDigits: 3 })}{' '}
                kW
              </span>
            </div>
            <div className="vh-mon__cell">
              <span className="vh-mon__k">{t('acPower')}</span>
              <span className="vh-mon__v">
                {peakAcKw.toLocaleString(undefined, { maximumFractionDigits: 3 })}{' '}
                kW
              </span>
            </div>
          </div>

          {station.inverter ? (
            <div className="vh-mon__block">
              <h3 className="vh-mon__h3">{t('inverter')}</h3>
              <table className="vh-mon__table">
                <tbody>
                  <tr>
                    <td>{t('inverterPower')}</td>
                    <td>{station.inverter.power} kW</td>
                  </tr>
                  <tr>
                    <td>{t('efficiency')}</td>
                    <td>{station.inverter.efficiency}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          ) : null}

          <div className="vh-mon__block">
            <h3 className="vh-mon__h3">{t('groupsTab')}</h3>
            <div className="vh-mon__groups">
              {station.panelGroups.map((g, i) => (
                <table key={g.id} className="vh-mon__table">
                  <caption className="vh-mon__cap">
                    {t('groupN')} {i + 1}
                  </caption>
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
                    <tr>
                      <td>P_dc</td>
                      <td>
                        {(
                          (g.panelCount * g.powerPerPanel) /
                          1000
                        ).toLocaleString(undefined, { maximumFractionDigits: 3 })}{' '}
                        kW
                      </td>
                    </tr>
                  </tbody>
                </table>
              ))}
            </div>
          </div>
        </section>
      ) : null}
    </div>
  )
}
