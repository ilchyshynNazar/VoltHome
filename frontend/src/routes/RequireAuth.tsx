import { Navigate, useLocation } from 'react-router-dom'
import type { ReactNode } from 'react'
import { useAuth } from '../context/AuthContext'

export function RequireAuth({ children }: { children: ReactNode }) {
  const { token } = useAuth()
  const loc = useLocation()

  if (!token) {
    return <Navigate to="/login" replace state={{ from: loc.pathname }} />
  }

  return children
}
