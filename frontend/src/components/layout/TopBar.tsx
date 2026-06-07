import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import { useI18n } from '../../context/I18nContext'
import { useStations } from '../../context/StationContext'
import { useTheme } from '../../context/ThemeContext'
import { FaSignOutAlt, FaMoon, FaSun } from 'react-icons/fa'
import { Button } from '../ui/Button'
import './TopBar.css'

export function TopBar() {
  const { t } = useI18n()
  const { logout } = useAuth()
  const { theme, toggleTheme } = useTheme()
  const { stations, selectedId, setSelectedId } = useStations()
  const navigate = useNavigate()

  const onLogout = () => {
    logout()
    navigate('/login', { replace: true })
  }

  return (
    <header className="vh-topbar">
      <div className="vh-topbar__spacer" />

      <div className="vh-topbar__right">
        {stations.length > 0 ? (
          <label className="vh-topbar__select-wrap">
            <span className="vh-topbar__select-label">{t('stationSelect')}</span>
            <select
              className="vh-topbar__select"
              value={selectedId ?? ''}
              onChange={(e) =>
                setSelectedId(e.target.value ? e.target.value : null)
              }
            >
              {stations.map((s) => (
                <option key={s.id} value={s.id}>
                  {s.name}
                </option>
              ))}
            </select>
          </label>
        ) : null}

        <Button
          variant="ghost"
          onClick={toggleTheme}
          title={theme === 'dark' ? t('themeLight') : t('themeDark')}
          aria-label={theme === 'dark' ? t('themeLight') : t('themeDark')}
          icon={theme === 'dark' ? <FaSun /> : <FaMoon />}
        />

        <Button
          variant="ghost"
          onClick={onLogout}
          icon={<FaSignOutAlt />}
          title={t('logout')}
          aria-label={t('logout')}
        />
      </div>
    </header>
  )
}
