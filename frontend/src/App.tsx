import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { AuthProvider } from './context/AuthContext'
import { I18nProvider } from './context/I18nContext'
import { StationProvider } from './context/StationContext'
import { ThemeProvider } from './context/ThemeContext'
import { ToastProvider } from './context/ToastContext'
import { AppLayout } from './components/layout/AppLayout'
import { ConfigPage } from './pages/ConfigPage'
import { LoginPage } from './pages/LoginPage'
import { MonitoringPage } from './pages/MonitoringPage'
import { GreenTariffPage } from './pages/GreenTariffPage'
import { PlaceholderPage } from './pages/PlaceholderPage'
import { RequireAuth } from './routes/RequireAuth'

export default function App() {
  return (
    <ThemeProvider>
      <I18nProvider>
        <ToastProvider>
          <AuthProvider>
            <BrowserRouter>
              <Routes>
                <Route path="/login" element={<LoginPage />} />
                <Route
                  element={
                    <RequireAuth>
                      <StationProvider>
                        <AppLayout />
                      </StationProvider>
                    </RequireAuth>
                  }
                >
                  <Route
                    path="/"
                    element={<Navigate to="/monitoring" replace />}
                  />
                  <Route path="/monitoring" element={<MonitoringPage />} />
                  <Route path="/config" element={<ConfigPage />} />
                  <Route
                    path="/payback"
                    element={
                      <PlaceholderPage
                        titleKey="payback"
                        messageKey="paybackSoon"
                      />
                    }
                  />
                  <Route path="/tariff" element={<GreenTariffPage />} />
                </Route>
                <Route
                  path="*"
                  element={<Navigate to="/monitoring" replace />}
                />
              </Routes>
            </BrowserRouter>
          </AuthProvider>
        </ToastProvider>
      </I18nProvider>
    </ThemeProvider>
  )
}
