import type { AuthResponseDto, LoginDto, RegisterDto } from '../types/auth';

export async function register(dto: RegisterDto): Promise<AuthResponseDto> {
  const res = await fetch('/api/auth/register', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error('Registration failed.');
  return res.json() as Promise<AuthResponseDto>;
}

export async function login(dto: LoginDto): Promise<AuthResponseDto> {
  const res = await fetch('/api/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error('Invalid email or password.');
  return res.json() as Promise<AuthResponseDto>;
}
