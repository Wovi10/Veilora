import type { AuthResponseDto, ForgotPasswordDto, LoginDto, RegisterDto, ResetPasswordDto } from '../types/auth';

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

export async function forgotPassword(dto: ForgotPasswordDto): Promise<void> {
  const res = await fetch('/api/auth/forgot-password', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error('Something went wrong. Please try again.');
}

export async function resetPassword(dto: ResetPasswordDto): Promise<void> {
  const res = await fetch('/api/auth/reset-password', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error('This reset link is invalid or has expired.');
}
