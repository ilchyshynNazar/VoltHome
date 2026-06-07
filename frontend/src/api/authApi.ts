import { apiFetch } from './http'

export interface LoginRequest {
  userName: string
  password: string
}

export interface LoginResponse {
  accessToken: string
  refreshToken: string
  refreshTokenValidTo: string
}

export interface RegisterRequest {
  userName: string
  email: string
  password: string
  phoneNumber: string
  firstName: string
  lastName: string
}

export function login(req: LoginRequest): Promise<LoginResponse> {
  return apiFetch<LoginResponse>('/api/Auth/login', {
    method: 'POST',
    json: req,
  })
}

export function registerClient(req: RegisterRequest): Promise<unknown> {
  return apiFetch('/api/Auth/register/client', {
    method: 'POST',
    json: req,
  })
}
