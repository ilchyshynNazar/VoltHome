import { describe, expect, it } from "vitest"
import type { CreateSolarStationPayload } from "../types/solar"

import {
  calculatePeakAcKw,
  calculatePeakDcKw,
  estimateAnnualKwh,
  estimateAnnualRevenueUah,
  estimateSystemCostUah,
  mapPayloadToStationDto,
  roughPaybackYears,
} from "./solarEconomics"

describe("solarEconomics utils", () => {
  const payload: CreateSolarStationPayload = {
    name: "Test Station",
    solarRegionId: "region-1",
    inverter: {
      power: 5000,
      efficiency: 98,
    },
    panelGroups: [
      {
        panelCount: 10,
        powerPerPanel: 400,
        tiltAngle: 20,
        azimuth: 180,
      },
      {
        panelCount: 5,
        powerPerPanel: 420,
        tiltAngle: 25,
        azimuth: 135,
      },
    ],
  }

  const monthlyProfile = Array.from({ length: 12 }, (_value, index) => ({
    month: index + 1,
    relativeCoefficient: 1,
    percentOfPeak: 100,
  }))

  it("maps payload to station DTO with id and region name", () => {
    const result = mapPayloadToStationDto(
      payload,
      "station-1",
      "Kyiv",
    )

    expect(result.id).toBe("station-1")
    expect(result.solarRegionName).toBe("Kyiv")
    expect(result.name).toBe(payload.name)
    expect(result.panelGroups).toHaveLength(2)

    expect(result.panelGroups[0].id).toBe("group-1")
    expect(result.panelGroups[1].id).toBe("group-2")
  })

  it("calculates DC peak power correctly for multiple groups", () => {
    expect(calculatePeakDcKw(payload.panelGroups)).toBe(
      10 * 400 / 1000 + 5 * 420 / 1000,
    )
  })

  it("calculates AC peak power using inverter efficiency", () => {
    const dcKw = 7.1

    expect(
      calculatePeakAcKw(dcKw, payload.inverter),
    ).toBe(Number((dcKw * 0.98).toFixed(3)))
  })

  it("returns zero AC peak power when inverter is missing", () => {
    expect(calculatePeakAcKw(5, null)).toBe(0)
  })

  it("estimates system cost using a fixed cost per kW", () => {
    expect(estimateSystemCostUah(4)).toBe(88000)
  })

  it("estimates annual production from monthly profile", () => {
    expect(estimateAnnualKwh(4, monthlyProfile)).toBe(4800)
  })

  it("estimates annual revenue from green tariff", () => {
    expect(estimateAnnualRevenueUah(4800)).toBe(23040)
  })

  it("returns infinite payback if annual revenue is zero", () => {
    expect(roughPaybackYears(100000, 0)).toBe(
      Number.POSITIVE_INFINITY,
    )
  })
})