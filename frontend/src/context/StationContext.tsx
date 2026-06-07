import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import type { SolarStationDto } from '../types/solar'
import { fetchStations } from '../api/solarApi'
import { useAuth } from './AuthContext'

const STORAGE_KEY = 'volthome-selected-station'

type StationContextValue = {
  stations: SolarStationDto[]
  selectedId: string | null
  setSelectedId: (id: string | null) => void
  reload: () => Promise<void>
  loading: boolean
}

const StationContext = createContext<StationContextValue | null>(null)

export function StationProvider({ children }: { children: ReactNode }) {
  const { token } = useAuth()
  const [stations, setStations] = useState<SolarStationDto[]>([])
  const [selectedId, setSelectedIdState] = useState<string | null>(() =>
    localStorage.getItem(STORAGE_KEY)
  )
  const [loading, setLoading] = useState(false)

  const reload = useCallback(async () => {
    if (!token) {
      setStations([])
      return
    }
    setLoading(true)
    try {
      const list = await fetchStations()
      setStations(list)
      const stored = localStorage.getItem(STORAGE_KEY)
      const valid =
        stored && list.some((s) => s.id === stored) ? stored : list[0]?.id
      if (valid) {
        setSelectedIdState(valid)
        localStorage.setItem(STORAGE_KEY, valid)
      } else {
        setSelectedIdState(null)
        localStorage.removeItem(STORAGE_KEY)
      }
    } finally {
      setLoading(false)
    }
  }, [token])

  useEffect(() => {
    void reload()
  }, [reload])

  const setSelectedId = useCallback((id: string | null) => {
    setSelectedIdState(id)
    if (id) localStorage.setItem(STORAGE_KEY, id)
    else localStorage.removeItem(STORAGE_KEY)
  }, [])

  const value = useMemo(
    () => ({ stations, selectedId, setSelectedId, reload, loading }),
    [stations, selectedId, setSelectedId, reload, loading]
  )

  return (
    <StationContext.Provider value={value}>{children}</StationContext.Provider>
  )
}

export function useStations(): StationContextValue {
  const ctx = useContext(StationContext)
  if (!ctx) throw new Error('useStations outside StationProvider')
  return ctx
}
