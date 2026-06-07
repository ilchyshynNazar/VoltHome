import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import { createPortal } from 'react-dom'
import './ToastContext.css'

type ToastType = 'success' | 'error'

type ToastItem = {
  id: string
  type: ToastType
  message: string
}

type ToastApi = {
  success: (message: string) => void
  error: (message: string) => void
}

const ToastContext = createContext<ToastApi | null>(null)

const DISMISS_MS = 5200

export function ToastProvider({ children }: { children: ReactNode }) {
  const [toasts, setToasts] = useState<ToastItem[]>([])

  const push = useCallback((message: string, type: ToastType) => {
    const id = crypto.randomUUID()
    setToasts((prev) => [...prev, { id, type, message }])
    window.setTimeout(() => {
      setToasts((prev) => prev.filter((t) => t.id !== id))
    }, DISMISS_MS)
  }, [])

  const success = useCallback(
    (message: string) => push(message, 'success'),
    [push]
  )
  const error = useCallback(
    (message: string) => push(message, 'error'),
    [push]
  )

  const dismiss = useCallback((id: string) => {
    setToasts((prev) => prev.filter((t) => t.id !== id))
  }, [])

  const value = useMemo(() => ({ success, error }), [success, error])

  return (
    <ToastContext.Provider value={value}>
      {children}
      {createPortal(
        <div className="vh-toast-host" aria-live="polite">
          {toasts.map((t) => (
            <div
              key={t.id}
              className={`vh-toast vh-toast--${t.type}`}
              role="status"
            >
              <span className="vh-toast__msg">{t.message}</span>
              <button
                type="button"
                className="vh-toast__close"
                aria-label="Close"
                onClick={() => dismiss(t.id)}
              >
                ×
              </button>
            </div>
          ))}
        </div>,
        document.body
      )}
    </ToastContext.Provider>
  )
}

export function useToast(): ToastApi {
  const ctx = useContext(ToastContext)
  if (!ctx) throw new Error('useToast outside ToastProvider')
  return ctx
}
