import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useMutation } from '@tanstack/react-query';
import { register } from '../../services';
import Background from '../../components/molecules/Background/Index';
import FormPanel from '../../components/molecules/FormPanel';

const Register = () => {
    const navigate = useNavigate();
    const [email, setEmail] = useState('');
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [errors, setErrors] = useState<Record<string, string[]>>({});
    const [formError, setFormError] = useState<string | null>(null);

    const registerMutation = useMutation({
        mutationFn: register,
        onSuccess: () => navigate('/login'),
        onError: (err: any) => {
            const data = err?.response?.data;

            if (data?.errors) {
                setErrors(data.errors);
            } else {
                setFormError(
                    data?.message
                    ?? data?.detail
                    ?? 'Something went wrong, please try again'
                );
            }
        },
    });

    const clearFieldError = (field: string) => {
        setErrors((prev) => {
            if (!prev[field]) return prev;
            const { [field]: _, ...rest } = prev;
            return rest;
        });
    };

    const handleSubmit = () => {
        setErrors({});
        setFormError(null);

        if (password !== confirmPassword) {
            setErrors({ ConfirmPassword: ['Passwords do not match'] });
            return;
        }

        registerMutation.mutate({ email, username, password });
    };

    const fieldClass = (field: string) =>
        `px-3 py-2 bg-black/50 text-sm text-gray-300 rounded focus:outline-none border ${
            errors[field]
                ? 'border-red-400 focus:border-red-300'
                : 'border-gray-400 focus:border-gray-200'
        }`;

    return (
        <div className="flex items-center justify-center min-h-[calc(100vh-4rem)]">
            <Background reveal={true} />
            <FormPanel className="min-h-full w-full flex flex-col justify-start gap-4">
                <h1 className="flex justify-center text-[46px] font-bold text-white pt-2">Register</h1>

                <div className="flex flex-col gap-8">
                    <div className="flex flex-col gap-4">
                        {formError && (
                            <p className="text-sm text-red-400">{formError}</p>
                        )}

                        <div className="flex flex-col gap-1.5">
                            <label htmlFor="email" className="flex justify-start font-medium text-white">
                                Email
                            </label>
                            <input
                                id="email"
                                type="email"
                                value={email}
                                onChange={(e) => { setEmail(e.target.value); clearFieldError('Email'); }}
                                placeholder="Enter your email"
                                aria-invalid={!!errors.Email}
                                className={fieldClass('Email')}
                            />
                            {errors.Email?.map((msg) => (
                                <p key={msg} className="text-xs text-red-400">{msg}</p>
                            ))}
                        </div>

                        <div className="flex flex-col gap-1.5">
                            <label htmlFor="username" className="flex justify-start font-medium text-white">
                                Username
                            </label>
                            <input
                                id="username"
                                type="text"
                                value={username}
                                onChange={(e) => { setUsername(e.target.value); clearFieldError('Username'); }}
                                placeholder="Enter your username"
                                aria-invalid={!!errors.Username}
                                className={fieldClass('Username')}
                            />
                            {errors.Username?.map((msg) => (
                                <p key={msg} className="text-xs text-red-400">{msg}</p>
                            ))}
                        </div>

                        <div className="flex flex-col gap-1.5">
                            <label htmlFor="password" className="flex justify-start font-medium text-white">
                                Password
                            </label>
                            <input
                                id="password"
                                type="password"
                                value={password}
                                onChange={(e) => { setPassword(e.target.value); clearFieldError('Password'); }}
                                placeholder="Enter your password"
                                aria-invalid={!!errors.Password}
                                className={fieldClass('Password')}
                            />
                            {errors.Password?.map((msg) => (
                                <p key={msg} className="text-xs text-red-400">{msg}</p>
                            ))}
                        </div>

                        <div className="flex flex-col gap-1.5">
                            <label htmlFor="confirmPassword" className="flex justify-start font-medium text-white">
                                Confirm Password
                            </label>
                            <input
                                id="confirmPassword"
                                type="password"
                                value={confirmPassword}
                                onChange={(e) => { setConfirmPassword(e.target.value); clearFieldError('ConfirmPassword'); }}
                                placeholder="Re-enter your password"
                                aria-invalid={!!errors.ConfirmPassword}
                                className={fieldClass('ConfirmPassword')}
                            />
                            {errors.ConfirmPassword?.map((msg) => (
                                <p key={msg} className="text-xs text-red-400">{msg}</p>
                            ))}
                        </div>
                    </div>

                    <div>
                        <button
                            onClick={handleSubmit}
                            disabled={registerMutation.isPending}
                            className="justify-center px-4 py-2 w-1/2 bg-[#930093] hover:bg-[#600060] text-sm text-white rounded disabled:opacity-50 disabled:cursor-not-allowed"
                        >
                            {registerMutation.isPending ? 'REGISTERING...' : 'REGISTER'}
                        </button>
                    </div>
                </div>

                <div className="flex h-full pb-12">
                    <p className="flex items-end justify-center w-full gap-4 text-sm text-center text-gray-400">
                        Already have an account?{' '}
                        <Link to="/login" className="font-medium text-blue-400 underline cursor-pointer">
                            Login
                        </Link>
                    </p>
                </div>
            </FormPanel>
        </div>
    );
};

export default Register;