import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useMutation } from '@tanstack/react-query';
import { register } from '../../services';

const Register = () => {
    const navigate = useNavigate();
    const [email, setEmail] = useState('');
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
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
        registerMutation.mutate({ email, username, password });
    };

    const fieldClass = (field: string) =>
        `px-3 py-2 text-sm border rounded focus:outline-none ${
            errors[field]
                ? 'border-red-400 focus:border-red-500'
                : 'border-gray-200 focus:border-gray-400'
        }`;

    return (
        <div className="flex items-center justify-center min-h-[calc(100vh-4rem)] px-4">
            <div className="flex flex-col gap-6 w-full max-w-sm p-8 border border-gray-200 rounded-lg">
                <h1 className="text-2xl font-bold text-gray-900">Register</h1>
                {formError && (
                    <p className="text-sm text-red-500">{formError}</p>
                )}
                <div className="flex flex-col gap-4">
                    <div className="flex flex-col gap-1.5">
                        <label htmlFor="email" className="text-sm font-medium text-gray-700">
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
                            <p key={msg} className="text-xs text-red-500">{msg}</p>
                        ))}
                    </div>
                    <div className="flex flex-col gap-1.5">
                        <label htmlFor="username" className="text-sm font-medium text-gray-700">
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
                            <p key={msg} className="text-xs text-red-500">{msg}</p>
                        ))}
                    </div>
                    <div className="flex flex-col gap-1.5">
                        <label htmlFor="password" className="text-sm font-medium text-gray-700">
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
                            <p key={msg} className="text-xs text-red-500">{msg}</p>
                        ))}
                    </div>
                    <button
                        onClick={handleSubmit}
                        disabled={registerMutation.isPending}
                        className="mt-2 px-4 py-2 text-sm font-medium border border-gray-900 rounded hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                        {registerMutation.isPending ? 'Registering...' : 'Register'}
                    </button>
                </div>
                <p className="text-sm text-center text-gray-500">
                    Already have an account?{' '}
                    <Link to="/login" className="font-medium text-gray-900 hover:underline">
                        Login
                    </Link>
                </p>
            </div>
        </div>
    );
};

export default Register;