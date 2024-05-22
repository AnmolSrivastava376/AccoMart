export interface RefreshToken {

    "accessToken": {
      "token": string,
      "expiryTokenDate": string
    },
    "refreshToken": {
      "token": string,
      "expiryTokenDate": string
    }

}

export interface RefreshTokenResponse {
  isSuccess: boolean;
  message: string;
  statusCode: number;
  response: {
    accessToken: {
      token: string;
      expiryTokenDate: string; 
    };
    refreshToken: {
      token: string;
      expiryTokenDate: string; 
    };
  };
}