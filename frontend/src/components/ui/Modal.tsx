import { useEffect, type ReactNode } from 'react'
import { createPortal } from 'react-dom'
import './Modal.css'

type Props = {
  open: boolean
  title: string
  onClose: () => void
  children: ReactNode
  footer?: ReactNode
  wide?: boolean
  /** Extra-wide layout for station editor */
  xl?: boolean
}

export function Modal({
  open,
  title,
  onClose,
  children,
  footer,
  wide,
  xl,
}: Props) {
  useEffect(() => {
    if (!open) return
    const onKey = (e: KeyboardEvent) => {
      if (e.key === 'Escape') onClose()
    }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  }, [open, onClose])

  if (!open) return null

  return createPortal(
    <div className="vh-modal-backdrop" role="presentation" onClick={onClose}>
      <div
        className={`vh-modal ${wide ? 'vh-modal--wide' : ''} ${xl ? 'vh-modal--xl' : ''}`.trim()}
        role="dialog"
        aria-modal="true"
        aria-labelledby="vh-modal-title"
        onClick={(e) => e.stopPropagation()}
      >
        <header className="vh-modal__head">
          <h2 id="vh-modal-title">{title}</h2>
          <button
            type="button"
            className="vh-modal__close"
            aria-label="Close"
            onClick={onClose}
          >
            ×
          </button>
        </header>
        <div className="vh-modal__body">{children}</div>
        {footer ? <footer className="vh-modal__foot">{footer}</footer> : null}
      </div>
    </div>,
    document.body
  )
}
