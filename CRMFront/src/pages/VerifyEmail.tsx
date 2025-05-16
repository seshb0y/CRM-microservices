import { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import axiosInstance from '../api/axiosInstance';
import { toast } from 'react-toastify';
import LoadingScreen from '../components/LoadingScreen';

const VerifyEmail = () => {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const verifyEmail = async () => {
            const token = searchParams.get('token');
            if (!token) {
                toast.error('No verification token found');
                navigate('/login');
                return;
            }

            try {
                await axiosInstance.get(`/account/email/verify?token=${token}`);
                toast.success('Email verified successfully! You can now log in.');
                navigate('/login');
            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            } catch (error: any) {
                if (error.response?.data?.message) {
                    toast.error(error.response.data.message);
                } else {
                    toast.error('Failed to verify email. The link may be expired or invalid.');
                }
                navigate('/login');
            } finally {
                setIsLoading(false);
            }
        };

        verifyEmail();
    }, [searchParams, navigate]);

    if (isLoading) {
        return <LoadingScreen title="Verifying Email" subtitle="Please wait while we verify your email address..." />;
    }

    return null;
};

export default VerifyEmail; 