export interface LoginDto {
  usernameOrEmail: string;
  password: string;
}

export interface RegisterDto {
  username: string;
  email: string;
  password: string;
  displayName?: string;
}

export interface AuthResponseDto {
  id: string;
  token: string;
  email: string;
  displayName?: string;
}
