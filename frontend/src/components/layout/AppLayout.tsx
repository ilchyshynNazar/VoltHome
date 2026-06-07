import { Outlet } from 'react-router-dom'
import { useI18n } from '../../context/I18nContext'
import { Sidebar } from './Sidebar'
import { TopBar } from './TopBar'
import './AppLayout.css'

export function AppLayout() {
  const { locale, setLocale, t } = useI18n()

  return (
    <div className="vh-shell">
      <Sidebar />
      <div className="vh-shell__main">
        <TopBar />
        <div className="vh-shell__content">
          <Outlet />
        </div>
        <footer className="vh-shell__footer">
          <label className="vh-lang">
            <span className="vh-lang__label">{t('language')}</span>
            <select
              className="vh-lang__select"
              value={locale}
              onChange={(e) => setLocale(e.target.value as 'uk' | 'en')}
            >
              <option value="uk">Українська</option>
              <option value="en">English</option>
            </select>
          </label>
        </footer>
      </div>
    </div>
  )
}
