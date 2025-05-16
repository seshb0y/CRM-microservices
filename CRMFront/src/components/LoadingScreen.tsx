import React from 'react';
import LoadingSpinner from './LoadingSpinner';

interface LoadingScreenProps {
  title: string;
  subtitle?: string;
}

const LoadingScreen: React.FC<LoadingScreenProps> = ({ title, subtitle }) => {
  return (
    <div className="flex-1 flex flex-col items-center justify-center w-screen min-h-screen bg-dark-bg">
      <div className="relative">
        <div className="absolute inset-0 bg-primary-purple/20 blur-3xl rounded-full" />
        <div className="relative z-10 flex flex-col items-center">
          <h1 className="text-3xl font-bold text-primary-purple mb-2">{title}</h1>
          {subtitle && <p className="text-gray-400 mb-6">{subtitle}</p>}
          <LoadingSpinner />
        </div>
      </div>
    </div>
  );
};

export default LoadingScreen; 