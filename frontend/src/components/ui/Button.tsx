import type { ButtonHTMLAttributes, ReactNode } from 'react'
import './Button.css'

type Variant = 'primary' | 'success' | 'danger' | 'ghost'

type Props = ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: Variant
  icon?: ReactNode
  children?: ReactNode
}

export function Button({
  variant = 'primary',
  icon,
  children,
  className = '',
  ...rest
}: Props) {
  return (
    <button
      type="button"
      className={`vh-btn vh-btn--${variant} ${className}`.trim()}
      {...rest}
    >
      {icon ? <span className="vh-btn__icon">{icon}</span> : null}
      {children ? <span className="vh-btn__text">{children}</span> : null}
    </button>
  )
}
