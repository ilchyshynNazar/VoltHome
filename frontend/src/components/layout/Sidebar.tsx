import { useCallback, useEffect, useState } from 'react'
import { NavLink } from 'react-router-dom'
import { useI18n } from '../../context/I18nContext'
import logoUrl from '../../assets/logo.svg'
import logoShortUrl from '../../assets/logo_short.svg'
import {
  FaChartBar,
  FaChartLine,
  FaChevronLeft,
  FaCog,
  FaLeaf,
} from 'react-icons/fa'
import './Sidebar.css'

const STORAGE_EXPANDED = 'volthome-sidebar-expanded'

export function Sidebar() {
  const { t } = useI18n()
  const [expanded, setExpanded] = useState(() =>
    localStorage.getItem(STORAGE_EXPANDED) === '1' ? true : false
  )

  useEffect(() => {
    localStorage.setItem(STORAGE_EXPANDED, expanded ? '1' : '0')
  }, [expanded])

  const toggle = useCallback(() => setExpanded((e) => !e), [])

  return (
    <aside className={`vh-sidebar ${expanded ? 'vh-sidebar--open' : ''}`}>
      <div className="vh-sidebar__inner">
        <div className="vh-sidebar__top">
          <div className="vh-sidebar__logo">
            <img src={expanded ? logoUrl : logoShortUrl} alt="VoltHome" />
          </div>
          <button
            type="button"
            className="vh-sidebar__toggle"
            onClick={toggle}
            title={expanded ? 'Collapse' : 'Expand'}
            aria-expanded={expanded}
          >
            <span
              className={`vh-sidebar__chev ${expanded ? 'vh-sidebar__chev--open' : ''}`}
            >
              <FaChevronLeft />
            </span>
          </button>
        </div>
        <nav className="vh-sidebar__nav">
          <NavLink to="/monitoring" className="vh-sidebar__link">
            <span className="vh-sidebar__ico">
              <FaChartLine />
            </span>
            <span className="vh-sidebar__label">{t('monitoring')}</span>
          </NavLink>
          <NavLink to="/config" className="vh-sidebar__link">
            <span className="vh-sidebar__ico">
              <FaCog />
            </span>
            <span className="vh-sidebar__label">{t('configuration')}</span>
          </NavLink>
          <NavLink to="/payback" className="vh-sidebar__link">
            <span className="vh-sidebar__ico">
              <FaChartBar />
            </span>
            <span className="vh-sidebar__label">{t('payback')}</span>
          </NavLink>
          <NavLink to="/tariff" className="vh-sidebar__link">
            <span className="vh-sidebar__ico">
              <FaLeaf />
            </span>
            <span className="vh-sidebar__label">{t('greenTariff')}</span>
          </NavLink>
        </nav>
      </div>
    </aside>
  )
}
