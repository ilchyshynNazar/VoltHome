import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import { STRINGS, type Locale, type MessageKey } from '../i18n/strings'

const STORAGE_KEY = 'volthome-locale'

type I18nContextValue = {
  locale: Locale
  setLocale: (l: Locale) => void
  t: (key: MessageKey) => string
}

const I18nContext = createContext<I18nContextValue | null>(null)

function readInitial(): Locale {
  const s = localStorage.getItem(STORAGE_KEY) as Locale | null
  if (s === 'uk' || s === 'en') return s
  return 'uk'
}

export function I18nProvider({ children }: { children: ReactNode }) {
  const [locale, setLocaleState] = useState<Locale>(readInitial)

  const setLocale = useCallback((l: Locale) => {
    setLocaleState(l)
    localStorage.setItem(STORAGE_KEY, l)
  }, [])

  const t = useCallback(
    (key: MessageKey) => STRINGS[locale][key] ?? key,
    [locale]
  )

  const value = useMemo(
    () => ({ locale, setLocale, t }),
    [locale, setLocale, t]
  )

  return <I18nContext.Provider value={value}>{children}</I18nContext.Provider>
}

export function useI18n(): I18nContextValue {
  const ctx = useContext(I18nContext)
  if (!ctx) throw new Error('useI18n outside I18nProvider')
  return ctx
}
