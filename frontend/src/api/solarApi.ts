import type {
  CreateSolarStationPayload,
  SolarDashboardDto,
  SolarPaybackInsight,
  SolarRegionOption,
  SolarStationDto,
  UpdateSolarStationPayload,
  CreateInverterPayload,
  CreatePanelGroupPayload,
} from '../types/solar'
import { apiFetch } from './http'

function mapStation(raw: Record<string, unknown>): SolarStationDto {
  const inv = (raw.inverter ?? raw.Inverter) as Record<string, unknown> | undefined
  const groups = raw.panelGroups ?? raw.PanelGroups
  return {
    id: String(raw.id),
    name: String(raw.name),
    solarRegionId: String(raw.solarRegionId ?? raw.SolarRegionId ?? ''),
    solarRegionName: String(
      raw.solarRegionName ?? raw.SolarRegionName ?? ''
    ),
    inverter: inv
      ? {
          power: Number(inv.power ?? inv.Power),
          efficiency: Number(inv.efficiency ?? inv.Efficiency),
        }
      : null,
    panelGroups: Array.isArray(groups)
      ? (groups as Record<string, unknown>[]).map((pg) => ({
          id: String(pg.id ?? pg.Id),
          panelCount: Number(pg.panelCount ?? pg.PanelCount),
          powerPerPanel: Number(pg.powerPerPanel ?? pg.PowerPerPanel),
          tiltAngle: Number(pg.tiltAngle ?? pg.TiltAngle),
          azimuth: Number(pg.azimuth ?? pg.Azimuth) as SolarStationDto['panelGroups'][0]['azimuth'],
        }))
      : [],
  }
}

export async function fetchRegions(): Promise<SolarRegionOption[]> {
  const list = await apiFetch<Record<string, unknown>[]>('/api/SolarRegions')
  return list.map((r) => ({
    id: String(r.id),
    name: String(r.name),
  }))
}

export async function fetchStations(): Promise<SolarStationDto[]> {
  const list = await apiFetch<Record<string, unknown>[]>('/api/SolarStations')
  return list.map(mapStation)
}

export async function fetchStation(id: string): Promise<SolarStationDto> {
  const raw = await apiFetch<Record<string, unknown>>(`/api/SolarStations/${id}`)
  return mapStation(raw)
}

export async function createStation(
  body: CreateSolarStationPayload
): Promise<{ id: string }> {
  return apiFetch<{ id: string }>('/api/SolarStations', {
    method: 'POST',
    json: {
      name: body.name,
      solarRegionId: body.solarRegionId,
      inverter: body.inverter,
      panelGroups: body.panelGroups.map((g) => ({
        panelCount: g.panelCount,
        powerPerPanel: g.powerPerPanel,
        tiltAngle: g.tiltAngle,
        azimuth: g.azimuth,
      })),
    },
  })
}

export function updateStation(
  id: string,
  body: UpdateSolarStationPayload
): Promise<void> {
  return apiFetch<void>(`/api/SolarStations/${id}`, {
    method: 'PUT',
    json: body,
  })
}

export function deleteStation(id: string): Promise<void> {
  return apiFetch<void>(`/api/SolarStations/${id}`, {
    method: 'DELETE',
  })
}

export function addPanelGroup(
  stationId: string,
  body: CreatePanelGroupPayload
): Promise<void> {
  return apiFetch<void>(`/api/SolarStations/${stationId}/panel-groups`, {
    method: 'POST',
    json: body,
  })
}

export function removePanelGroup(
  stationId: string,
  groupId: string
): Promise<void> {
  return apiFetch<void>(
    `/api/SolarStations/${stationId}/panel-groups/${groupId}`,
    { method: 'DELETE' }
  )
}

export function updateInverter(
  stationId: string,
  body: CreateInverterPayload
): Promise<void> {
  return apiFetch<void>(`/api/SolarStations/${stationId}/inverter`, {
    method: 'PUT',
    json: body,
  })
}

export function updatePanelGroup(
  stationId: string,
  groupId: string,
  body: CreatePanelGroupPayload
): Promise<void> {
  return apiFetch<void>(
    `/api/SolarStations/${stationId}/panel-groups/${groupId}`,
    { method: 'PUT', json: body }
  )
}

export async function fetchDashboard(
  stationId: string
): Promise<SolarDashboardDto> {
  const raw = await apiFetch<Record<string, unknown>>(
    `/api/solar/${stationId}/dashboard`
  )

  return {
    currentHourKwh: Number(raw.currentHourKwh ?? raw.CurrentHourKwh ?? 0),
    todayKwh: Number(raw.todayKwh ?? raw.TodayKwh ?? 0),

    forecastNextHourKwh: Number(
      raw.forecastNextHourKwh ?? raw.ForecastNextHourKwh ?? 0
    ),

    forecastTodayRemainingKwh: Number(
      raw.forecastTodayRemainingKwh ?? raw.ForecastTodayRemainingKwh ?? 0
    ),

    hourGaugeMax: Number(raw.hourGaugeMax ?? raw.HourGaugeMax ?? 1),
    dayGaugeMax: Number(raw.dayGaugeMax ?? raw.DayGaugeMax ?? 1),

    forecastGaugeMax: Number(
      raw.forecastGaugeMax ?? raw.ForecastGaugeMax ?? 1
    ),
  }
}

export async function fetchSolarInsights(
  stationId: string
): Promise<SolarPaybackInsight> {
  const raw = await apiFetch<Record<string, unknown>>(
    `/api/solar/${stationId}/insights`
  )
  const profile = (raw.monthlyProfile ?? raw.MonthlyProfile) as
    | Record<string, unknown>[]
    | undefined
  return {
    peakDcKw: Number(raw.peakDcKw ?? raw.PeakDcKw ?? 0),
    peakAcKw: Number(raw.peakAcKw ?? raw.PeakAcKw ?? 0),
    estimatedAnnualKwh: Number(
      raw.estimatedAnnualKwh ?? raw.EstimatedAnnualKwh ?? 0
    ),
    estimatedSystemCostUah: Number(
      raw.estimatedSystemCostUah ?? raw.EstimatedSystemCostUah ?? 0
    ),
    assumedGreenTariffUahPerKwh: Number(
      raw.assumedGreenTariffUahPerKwh ?? raw.AssumedGreenTariffUahPerKwh ?? 0
    ),
    estimatedAnnualRevenueUah: Number(
      raw.estimatedAnnualRevenueUah ?? raw.EstimatedAnnualRevenueUah ?? 0
    ),
    roughPaybackYears: Number(
      raw.roughPaybackYears ?? raw.RoughPaybackYears ?? 0
    ),
    disclaimer: String(raw.disclaimer ?? raw.Disclaimer ?? ''),
    monthlyProfile: Array.isArray(profile)
      ? profile.map((p) => ({
          month: Number(p.month ?? p.Month),
          relativeCoefficient: Number(
            p.relativeCoefficient ?? p.RelativeCoefficient ?? 0
          ),
          percentOfPeak: Number(p.percentOfPeak ?? p.PercentOfPeak ?? 0),
        }))
      : [],
  }
}
