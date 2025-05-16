import axios from "axios";
import { toast } from 'react-toastify';

interface ValidationErrors {
  [key: string]: string[];
}

interface ErrorResponse {
  message?: string;
  errors?: ValidationErrors;
}

const isDevelopment = import.meta.env.DEV;
const apiUrl = isDevelopment 
  ? import.meta.env.VITE_API_URL 
  : import.meta.env.VITE_PRODUCTION_API_URL;

console.log('API URL:', apiUrl);

const axiosInstance = axios.create({
  baseURL: apiUrl,
  withCredentials: true,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  }
});

let isRefreshing = false;
let failedQueue: { resolve: (value?: unknown) => void; reject: (reason?: unknown) => void; }[] = [];

const processQueue = (error: Error | null) => {
  failedQueue.forEach(prom => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve();
    }
  });
  failedQueue = [];
};

axiosInstance.interceptors.request.use(
  (config) => {
    if (config.method === 'get') {
      config.params = {
        ...config.params,
        _t: Date.now()
      };
    }
    return config;
  },
  (error) => {
    console.error("Request error:", error);
    toast.error('Error when sending request');
    return Promise.reject(error);
  }
);

axiosInstance.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
          .then(() => {
            return axiosInstance(originalRequest);
          })
          .catch(err => {
            return Promise.reject(err);
          });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      try {
        console.log('Refreshing token');
        await axiosInstance.post('/auth/refresh');
        processQueue(null);
        return axiosInstance(originalRequest);
      } catch (refreshError) {
        processQueue(refreshError as Error);
        localStorage.removeItem('isLogin');
        window.location.href = "/login";
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    if (!error.response) {
      toast.error('Server is not available. Please check your internet connection');
      return Promise.reject(error);
    }

    const errorData = error.response.data as ErrorResponse | string[];

    switch (error.response.status) {
      case 400:
        if (Array.isArray(errorData)) {
          errorData.forEach((errorMessage: string) => {
            toast.error(errorMessage);
          });
        } else if (typeof errorData === 'object' && errorData.errors) {
          Object.values(errorData.errors).forEach((errorMessages: string[]) => {
            errorMessages.forEach((message: string) => {
              toast.error(message);
            });
          });
        } else {
          toast.error((errorData as ErrorResponse).message || 'Validation error');
        }
        break;
      case 403:
        toast.error('You do not have permission to perform this action');
        break;
      case 404:
        toast.error('Requested resource not found');
        break;
      case 500:
        toast.error('Internal server error');
        break;
      default:
        toast.error(error.response.data?.message || 'An error occurred');
    }

    return Promise.reject(error);
  }
);

export default axiosInstance;
