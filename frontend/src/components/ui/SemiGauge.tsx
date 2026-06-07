import './SemiGauge.css'
import { useEffect, useState } from 'react'

type Props = {
  value: number
  max: number
  label: string
  sublabel?: string
  large?: boolean
}

function formatEnergyKwh(kwh: number): { text: string; unit: string } {
  if (!Number.isFinite(kwh) || kwh === 0) return { text: '0', unit: 'kWh' }
  if (kwh < 0.01)
    return {
      text: Math.round(kwh * 1000).toLocaleString(),
      unit: 'Wh',
    }
  if (kwh < 1)
    return {
      text: kwh.toLocaleString(undefined, { maximumFractionDigits: 3 }),
      unit: 'kWh',
    }
  return {
    text: kwh.toLocaleString(undefined, { maximumFractionDigits: 2 }),
    unit: 'kWh',
  }
}

export function SemiGauge({
  value,
  max,
  label,
  sublabel,
  large,
}: Props) {
  const [animatedRatio, setAnimatedRatio] = useState(0)

  useEffect(() => {
    const safeMax = max > 0 ? max : 1
    const target = Math.min(1, Math.max(0, value / safeMax))

    let frame: number
    const start = performance.now()
    const duration = 900

    const animate = (time: number) => {
      const progress = Math.min(1, (time - start) / duration)
      const eased = 1 - Math.pow(1 - progress, 3)

      setAnimatedRatio(eased * target)

      if (progress < 1) {
        frame = requestAnimationFrame(animate)
      }
    }

    frame = requestAnimationFrame(animate)

    return () => cancelAnimationFrame(frame)
  }, [value, max])

  const safeMax = max > 0 ? max : 1
  const ratio = animatedRatio

  const cx = 100
  const cy = 88
  const r = 72

  const start = Math.PI
  const sweep = Math.PI * ratio

  const x1 = cx + r * Math.cos(start)
  const y1 = cy - r * Math.sin(start)

  const x2 = cx + r * Math.cos(start - sweep)
  const y2 = cy - r * Math.sin(start - sweep)

  const largeArc = sweep > Math.PI ? 1 : 0

  const valuePath = `M ${x1} ${y1} A ${r} ${r} 0 ${largeArc} 1 ${x2} ${y2}`

  const needleAngle = Math.PI * (1 - ratio)
  const needleLen = 58

  const nx = cx + needleLen * Math.cos(needleAngle)
  const ny = cy - needleLen * Math.sin(needleAngle)

  const trackPath = `M ${cx - r} ${cy} A ${r} ${r} 0 0 1 ${cx + r} ${cy}`

  const { text, unit } = formatEnergyKwh(value)
  const maxFmt = formatEnergyKwh(safeMax)


  return (
    <div className={`vh-gauge ${large ? 'vh-gauge--lg' : ''}`}>
      <svg viewBox="0 0 200 118" className="vh-gauge__svg" aria-hidden>
        <path
          d={trackPath}
          fill="none"
          stroke="var(--vh-gauge-track)"
          strokeWidth="10"
          strokeLinecap="round"
        />
        <path
          d={valuePath}
          fill="none"
          stroke="url(#vhGaugeGrad)"
          strokeWidth="10"
          strokeLinecap="round"
        />
        <line
          x1={cx}
          y1={cy}
          x2={nx}
          y2={ny}
          stroke="var(--vh-text-strong)"
          strokeWidth="3"
          strokeLinecap="round"
        />
        <circle cx={cx} cy={cy} r="5" fill="var(--vh-accent)" />
        <defs>
          <linearGradient id="vhGaugeGrad" x1="0%" y1="0%" x2="100%" y2="0%">
            <stop offset="0%" stopColor="#22c55e" />
            <stop offset="100%" stopColor="#863bff" />
          </linearGradient>
        </defs>
      </svg>
      <div className="vh-gauge__readout">
        <span className="vh-gauge__value">{text}</span>
        <span className="vh-gauge__unit">{unit}</span>
      </div>
      <div className="vh-gauge__label">{label}</div>
      <div className="vh-gauge__sub">
        {sublabel ??
          `max ≈ ${maxFmt.text} ${maxFmt.unit}`}
      </div>
    </div>
  )
}
