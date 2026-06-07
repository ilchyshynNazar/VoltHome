const TOKEN_KEY = 'volthome_access_token'

export function getStoredToken(): string | null {
  return localStorage.getItem(TOKEN_KEY)
}

export function setStoredToken(token: string): void {
  localStorage.setItem(TOKEN_KEY, token)
}

export function clearStoredToken(): void {
  localStorage.removeItem(TOKEN_KEY)
}

export class ApiError extends Error {
  status: number

  constructor(message: string, status: number) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

/** Turns API response bodies (JSON ProblemDetails, validation errors, plain text) into a single readable string. */
export function parseApiErrorBody(text: string, status: number): string {
  const trimmed = text.trim()
  if (!trimmed) {
    if (status === 401) return 'Unauthorized'
    return `Request failed (${status})`
  }

  try {
    const j = JSON.parse(trimmed) as Record<string, unknown>

    if (typeof j.detail === 'string' && j.detail) return j.detail

    if (j.errors && typeof j.errors === 'object' && !Array.isArray(j.errors)) {
      const errs = j.errors as Record<string, string[] | string | unknown>
      const lines: string[] = []
      for (const [, v] of Object.entries(errs)) {
        if (Array.isArray(v)) {
          for (const msg of v) {
            if (typeof msg === 'string') lines.push(msg)
          }
        } else if (typeof v === 'string') lines.push(v)
      }
      if (lines.length) return [...new Set(lines)].join(' · ')
    }

    if (typeof j.title === 'string' && j.title) {
      const title = j.title
      if (title !== 'One or more validation errors occurred.') return title
    }

    if (typeof j.message === 'string') return j.message
    if (typeof j.Message === 'string') return j.Message
  } catch {
    /* not JSON */
  }

  if (trimmed.length > 600) return `${trimmed.slice(0, 600)}…`
  return trimmed
}

export function getErrorMessage(err: unknown): string {
  if (err instanceof ApiError) return err.message
  if (err instanceof Error) return err.message
  return 'Unknown error'
}

type FetchOpts = RequestInit & { json?: unknown }

export async function apiFetch<T>(path: string, init?: FetchOpts): Promise<T> {
  const headers: Record<string, string> = {
    ...(init?.headers as Record<string, string> | undefined),
  }
  const token = getStoredToken()
  if (token) headers.Authorization = `Bearer ${token}`

  let body: BodyInit | undefined = init?.body as BodyInit | undefined
  if (init?.json !== undefined) {
    headers['Content-Type'] = 'application/json'
    body = JSON.stringify(init.json)
  }

  const res = await fetch(path, { ...init, headers, body })

  if (res.status === 401) {
    clearStoredToken()
    window.dispatchEvent(new CustomEvent('volthome:unauthorized'))
  }

  if (!res.ok) {
    const text = await res.text()
    const msg = parseApiErrorBody(text, res.status)
    throw new ApiError(msg, res.status)
  }

  if (res.status === 204) return undefined as T

  return res.json() as Promise<T>
}
