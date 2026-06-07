import { type FormEvent, useEffect, useState } from 'react'
import { Eye, EyeOff } from 'lucide-react'
import { Navigate, useLocation } from 'react-router-dom'
import { getErrorMessage } from '../api/http'
import { registerClient } from '../api/authApi'
import { useAuth } from '../context/AuthContext'
import { useI18n } from '../context/I18nContext'
import { useToast } from '../context/ToastContext'
import backUrl from '../assets/back_logo.svg'
import { Button } from '../components/ui/Button'
import './LoginPage.css'

export function LoginPage() {
  const { t } = useI18n()
  const toast = useToast()
  const { token, login } = useAuth()
  const loc = useLocation()
  const from =
    (loc.state as { from?: string } | null)?.from &&
    (loc.state as { from?: string }).from !== '/login'
      ? (loc.state as { from: string }).from
      : '/monitoring'

  const [userName, setUserName] = useState('')
  const [password, setPassword] = useState('')
  const [loading, setLoading] = useState(false)
  const [showPassword, setShowPassword] = useState(false)
  const [showRegPassword, setShowRegPassword] = useState(false)

  const [showRegister, setShowRegister] = useState(false)
  const [reg, setReg] = useState({
    userName: '',
    email: '',
    password: '',
    phoneNumber: '+380',
    firstName: '',
    lastName: '',
  })
  const [regLoading, setRegLoading] = useState(false)

  if (token) {
    return <Navigate to={from} replace />
  }

  useEffect(() => {
    if (!showPassword) return

    const timer = setTimeout(() => {
      setShowPassword(false)
    }, 4000)

    return () => clearTimeout(timer)
  }, [showPassword])

  useEffect(() => {
    if (!showRegPassword) return

    const timer = setTimeout(() => {
      setShowRegPassword(false)
    }, 4000)

    return () => clearTimeout(timer)
  }, [showRegPassword])

  const onLogin = async (e: FormEvent) => {
    e.preventDefault()
    setLoading(true)
    try {
      await login({ userName, password })
      toast.success(t('loginSuccess'))
    } catch (err) {
      toast.error(getErrorMessage(err))
    } finally {
      setLoading(false)
    }
  }

  const onRegister = async (e: FormEvent) => {
    e.preventDefault()
    setRegLoading(true)
    try {
      const phoneNumber = reg.phoneNumber.replace(/\s/g, '')
      await registerClient({ ...reg, phoneNumber })
      toast.success(t('registerSuccess'))
      setShowRegister(false)
      setUserName(reg.userName)
      setPassword(reg.password)
    } catch (err) {
      toast.error(getErrorMessage(err))
    } finally {
      setRegLoading(false)
    }
  }

  return (
    <div className="vh-login">
      <div
        className="vh-login__bg"
        style={{ backgroundImage: `url(${backUrl})` }}
        aria-hidden
      />
      <div className="vh-login__panel">
        {!showRegister ? (
          <>
            <h1 className="vh-login__title">{t('appTitle')}</h1>
            <form className="vh-login__form" onSubmit={onLogin}>
              <label className="vh-field">
                <span>{t('userName')}</span>
                <input
                  className="vh-input"
                  autoComplete="username"
                  value={userName}
                  onChange={(e) => setUserName(e.target.value)}
                  required
                />
              </label>
              <label className="vh-field">
                <span>{t('password')}</span>
                <div className="vh-password">
                  <input
                    className="vh-input vh-password__input"
                    type={showPassword ? 'text' : 'password'}
                   autoComplete="current-password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                  />
                  <button
                    type="button"
                    className="vh-password__toggle"
                    onClick={() => setShowPassword((v) => !v)}
                  >
                    {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
                  </button>
                </div>
              </label>
              <Button
                variant="primary"
                type="submit"
                className="vh-login__submit"
                disabled={loading}
              >
                {t('login')}
              </Button>
            </form>
            <button
              type="button"
              className="vh-login__switch"
              onClick={() => setShowRegister(true)}
            >
              {t('register')}
            </button>
          </>
        ) : (
          <>
            <h1 className="vh-login__title">{t('register')}</h1>
            <form className="vh-login__form" onSubmit={onRegister}>
              <label className="vh-field">
                <span>{t('userName')}</span>
                <input
                  className="vh-input"
                  value={reg.userName}
                  onChange={(e) =>
                    setReg((r) => ({ ...r, userName: e.target.value }))
                  }
                  required
                  minLength={3}
                />
              </label>
              <label className="vh-field">
                <span>{t('email')}</span>
                <input
                  className="vh-input"
                  type="email"
                  value={reg.email}
                  onChange={(e) =>
                    setReg((r) => ({ ...r, email: e.target.value }))
                  }
                  required
                />
              </label>
              <label className="vh-field">
                <span>{t('password')}</span>
                <div className="vh-password">
                  <input
                    className="vh-input vh-password__input"
                    type={showRegPassword ? 'text' : 'password'}
                    value={reg.password}
                    onChange={(e) =>
                      setReg((r) => ({ ...r, password: e.target.value }))
                    }
                    required
                    minLength={8}
                  />
                  <button
                    type="button"
                    className="vh-password__toggle"
                    onClick={() => setShowRegPassword((v) => !v)}
                  >
                    {showRegPassword ? <EyeOff size={18} /> : <Eye size={18} />}
                  </button>
                </div>
              </label>
              <label className="vh-field">
                <span>{t('phone')}</span>
                <input
                  className="vh-input"
                  value={reg.phoneNumber}
                  onChange={(e) =>
                    setReg((r) => ({ ...r, phoneNumber: e.target.value }))
                  }
                  required
                  placeholder="+380501234567"
                />
              </label>
              <label className="vh-field">
                <span>{t('firstName')}</span>
                <input
                  className="vh-input"
                  value={reg.firstName}
                  onChange={(e) =>
                    setReg((r) => ({ ...r, firstName: e.target.value }))
                  }
                  required
                />
              </label>
              <label className="vh-field">
                <span>{t('lastName')}</span>
                <input
                  className="vh-input"
                  value={reg.lastName}
                  onChange={(e) =>
                    setReg((r) => ({ ...r, lastName: e.target.value }))
                  }
                  required
                />
              </label>
              <div className="vh-login__actions">
                <Button
                  type="button"
                  variant="ghost"
                  onClick={() => setShowRegister(false)}
                >
                  {t('cancel')}
                </Button>
                <Button variant="success" type="submit" disabled={regLoading}>
                  {t('register')}
                </Button>
              </div>
            </form>
          </>
        )}
      </div>
    </div>
  )
}
