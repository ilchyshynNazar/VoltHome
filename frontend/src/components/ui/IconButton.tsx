import type { ButtonHTMLAttributes, ReactNode } from 'react'
import './IconButton.css'

type Props = ButtonHTMLAttributes<HTMLButtonElement> & {
  icon: ReactNode
  label: string
  variant?: 'default' | 'danger' | 'success'
}

export function IconButton({
  icon,
  label,
  variant = 'default',
  className = '',
  type = 'button',
  ...rest
}: Props) {
  return (
    <button
      type={type}
      className={`vh-ibtn vh-ibtn--${variant} ${className}`.trim()}
      aria-label={label}
      title={label}
      {...rest}
    >
      <span className="vh-ibtn__ico">{icon}</span>
    </button>
  )
}
