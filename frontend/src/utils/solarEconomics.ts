import { SolarEconomicsOptions } from "../constants/solarEconomics"
import type {
  CreateSolarStationPayload,
  InverterDto,
  MonthlyIrradiationPoint,
  PanelGroupDto,
  SolarPaybackInsight,
  SolarStationDto,
} from "../types/solar"

export function mapPayloadToStationDto(
  payload: CreateSolarStationPayload,
  id: string,
  solarRegionName: string,
): SolarStationDto {
  return {
    id,
    name: payload.name,
    solarRegionId: payload.solarRegionId,
    solarRegionName,
    inverter: payload.inverter,
    panelGroups: payload.panelGroups.map((group, index) => ({
      id: `group-${index + 1}`,
      panelCount: group.panelCount,
      powerPerPanel: group.powerPerPanel,
      tiltAngle: group.tiltAngle,
      azimuth: group.azimuth,
    })),
  }
}

function round(value: number, digits = 3): number {
  return Number(value.toFixed(digits))
}

export function calculatePeakDcKw(
  panelGroups: Pick<PanelGroupDto, "panelCount" | "powerPerPanel">[],
): number {
  return round(
    panelGroups.reduce(
      (sum, group) => sum + (group.panelCount * group.powerPerPanel) / 1000,
      0,
    ),
  )
}

export function calculatePeakAcKw(
  peakDcKw: number,
  inverter: InverterDto | null,
): number {
  if (!inverter) {
    return 0
  }

  return round((peakDcKw * inverter.efficiency) / 100)
}

export function estimateSystemCostUah(peakDcKw: number): number {
  return round(peakDcKw * SolarEconomicsOptions.CostPerKwInstalledUah, 2)
}

export function calculateAnnualProductionFactor(
  monthlyProfile: MonthlyIrradiationPoint[],
): number {
  if (monthlyProfile.length === 0) {
    return 0
  }

  const averageCoefficient =
    monthlyProfile.reduce((sum, item) => sum + item.relativeCoefficient, 0) /
    monthlyProfile.length

  return round(averageCoefficient * 1200, 2)
}

export function estimateAnnualKwh(
  peakDcKw: number,
  monthlyProfile: MonthlyIrradiationPoint[],
): number {
  return round(peakDcKw * calculateAnnualProductionFactor(monthlyProfile), 0)
}

export function estimateAnnualRevenueUah(annualKwh: number): number {
  return round(annualKwh * SolarEconomicsOptions.GreenTariffUahPerKwh, 2)
}

export function roughPaybackYears(
  estimatedSystemCostUah: number,
  estimatedAnnualRevenueUah: number,
): number {
  if (estimatedAnnualRevenueUah <= 0) {
    return Number.POSITIVE_INFINITY
  }

  return round(estimatedSystemCostUah / estimatedAnnualRevenueUah, 2)
}

export function createSolarPaybackInsight(
  payload: CreateSolarStationPayload,
  monthlyProfile: MonthlyIrradiationPoint[],
) {
  const peakDcKw = calculatePeakDcKw(payload.panelGroups)
  const peakAcKw = calculatePeakAcKw(peakDcKw, payload.inverter)
  const estimatedAnnualKwh = estimateAnnualKwh(peakDcKw, monthlyProfile)
  const estimatedSystemCostUah = estimateSystemCostUah(peakDcKw)
  const estimatedAnnualRevenueUah = estimateAnnualRevenueUah(
    estimatedAnnualKwh,
  )

  return {
    peakDcKw,
    peakAcKw,
    estimatedAnnualKwh,
    estimatedSystemCostUah,
    assumedGreenTariffUahPerKwh:
      SolarEconomicsOptions.GreenTariffUahPerKwh,
    estimatedAnnualRevenueUah,
    roughPaybackYears: roughPaybackYears(
      estimatedSystemCostUah,
      estimatedAnnualRevenueUah,
    ),
    monthlyProfile,
    disclaimer: SolarEconomicsOptions.Disclaimer,
  } satisfies SolarPaybackInsight
}