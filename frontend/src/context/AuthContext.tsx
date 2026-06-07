import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import { clearStoredToken, getStoredToken, setStoredToken } from '../api/http'
import { login as loginApi, type LoginRequest } from '../api/authApi'

const USER_KEY = 'volthome_user_display'

type AuthContextValue = {
  token: string | null
  userLabel: string | null
  login: (req: LoginRequest) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() => getStoredToken())
  const [userLabel, setUserLabel] = useState<string | null>(() =>
    localStorage.getItem(USER_KEY)
  )

  useEffect(() => {
    const onUnauth = () => {
      setToken(null)
      setUserLabel(null)
    }
    window.addEventListener('volthome:unauthorized', onUnauth)
    return () => window.removeEventListener('volthome:unauthorized', onUnauth)
  }, [])

  const login = useCallback(async (req: LoginRequest) => {
    const res = await loginApi(req)
    setStoredToken(res.accessToken)
    setToken(res.accessToken)
    localStorage.setItem(USER_KEY, req.userName)
    setUserLabel(req.userName)
  }, [])

  const logout = useCallback(() => {
    clearStoredToken()
    localStorage.removeItem(USER_KEY)
    setToken(null)
    setUserLabel(null)
  }, [])

  const value = useMemo(
    () => ({ token, userLabel, login, logout }),
    [token, userLabel, login, logout]
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth outside AuthProvider')
  return ctx
}
