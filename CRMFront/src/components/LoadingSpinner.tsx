import React from "react";
import Lottie from "lottie-react";
import loadAnimation from "../assets/LoadAnimation.json";

const LoadingSpinner: React.FC = () => {
  return (
    <div className="relative mb-6 w-40 h-40">
      <Lottie animationData={loadAnimation} loop={true} />
    </div>
  );
};

export default LoadingSpinner;
