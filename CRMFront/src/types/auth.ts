export interface ResetPasswordRequest {
  username: string;
}

export interface ChangePasswordRequest {
  token: string;
  newPassword: string;
} 