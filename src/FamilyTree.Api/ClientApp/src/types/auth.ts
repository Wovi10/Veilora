export interface LoginDto {
  usernameOrEmail: string;
  password: string;
}

export interface AuthResponseDto {
  token: string;
  email: string;
  displayName?: string;
}
