export type SolarAzimuth =
  | 0
  | 45
  | 90
  | 135
  | 180
  | 225
  | 270
  | 315

export const SOLAR_AZIMUTHS: SolarAzimuth[] = [
  0, 45, 90, 135, 180, 225, 270, 315,
]

export interface InverterDto {
  power: number
  efficiency: number
}

export interface PanelGroupDto {
  id: string
  panelCount: number
  powerPerPanel: number
  tiltAngle: number
  azimuth: SolarAzimuth
}

export interface SolarStationDto {
  id: string
  name: string
  solarRegionId: string
  solarRegionName: string
  inverter: InverterDto | null
  panelGroups: PanelGroupDto[]
}

export interface SolarDashboardDto {
  currentHourKwh: number
  todayKwh: number

  forecastNextHourKwh: number
  forecastTodayRemainingKwh: number

  hourGaugeMax: number
  dayGaugeMax: number
  forecastGaugeMax: number
}

export interface MonthlyIrradiationPoint {
  month: number
  relativeCoefficient: number
  percentOfPeak: number
}

export interface SolarPaybackInsight {
  peakDcKw: number
  peakAcKw: number
  estimatedAnnualKwh: number
  estimatedSystemCostUah: number
  assumedGreenTariffUahPerKwh: number
  estimatedAnnualRevenueUah: number
  roughPaybackYears: number
  monthlyProfile: MonthlyIrradiationPoint[]
  disclaimer: string
}

export interface SolarRegionOption {
  id: string
  name: string
}

export interface CreateInverterPayload {
  power: number
  efficiency: number
}

export interface CreatePanelGroupPayload {
  panelCount: number
  powerPerPanel: number
  tiltAngle: number
  azimuth: SolarAzimuth
}

export interface CreateSolarStationPayload {
  name: string
  solarRegionId: string
  inverter: CreateInverterPayload
  panelGroups: CreatePanelGroupPayload[]
}

export interface UpdateSolarStationPayload {
  name: string
  solarRegionId: string
}
