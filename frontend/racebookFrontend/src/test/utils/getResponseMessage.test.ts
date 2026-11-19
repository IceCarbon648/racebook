import { describe, it, expect } from 'vitest';
import { AxiosError, type AxiosResponse } from 'axios';
import { getResponseMessage, getResponseStatus } from '../../utils/getResponseMessage';

const DEFAULT_FALLBACK = 'Something went wrong, please try again';
const CUSTOM_FALLBACK = 'Could not delete the mod';

const makeAxiosError = (status?: number, data?: unknown): AxiosError => {
    const error = new AxiosError('Request failed');

    if (status !== undefined) {
        error.response = {
            data,
            status,
            statusText: '',
            headers: {},
            config: {} as never,
        } as AxiosResponse;
    }

    return error;
};

describe('getResponseMessage', () => {
    it('falls back when the error did not come from axios', () => {
        expect(getResponseMessage(new Error('boom'))).toBe(DEFAULT_FALLBACK);
        expect(getResponseMessage('boom')).toBe(DEFAULT_FALLBACK);
        expect(getResponseMessage(new Error('boom'), CUSTOM_FALLBACK)).toBe(CUSTOM_FALLBACK);
    });

    it('falls back when the request never reached the server', () => {
        expect(getResponseMessage(makeAxiosError())).toBe(DEFAULT_FALLBACK);
    });

    it('joins every field error from a validation response', () => {
        const error = makeAxiosError(400, {
            errors: {
                Email: ['Email is required'],
                Password: ['Too short', 'Needs a number'],
            },
        });

        expect(getResponseMessage(error)).toBe('Email is required Too short Needs a number');
    });

    it('prefers field errors, then the message, then the detail', () => {
        const withEverything = makeAxiosError(400, {
            errors: { Email: ['Email is required'] },
            message: 'Validation failed',
            detail: 'An unexpected error occurred',
        });

        const withoutFieldErrors = makeAxiosError(409, {
            message: 'Registration failed',
            detail: 'An unexpected error occurred',
        });

        const withOnlyDetail = makeAxiosError(500, {
            title: 'An error occurred',
            detail: 'An unexpected error occurred',
        });

        expect(getResponseMessage(withEverything)).toBe('Email is required');
        expect(getResponseMessage(withoutFieldErrors)).toBe('Registration failed');
        expect(getResponseMessage(withOnlyDetail)).toBe('An unexpected error occurred');
    });

    it('falls back when the response body carries nothing usable', () => {
        expect(getResponseMessage(makeAxiosError(500, {}))).toBe(DEFAULT_FALLBACK);
        expect(getResponseMessage(makeAxiosError(502, 'Bad Gateway'))).toBe(DEFAULT_FALLBACK);
    });

    it('falls back when the errors object is present but empty', () => {
        const error = makeAxiosError(400, { errors: {}, message: 'Validation failed' });

        expect(getResponseMessage(error)).toBe('Validation failed');
    });
});

describe('getResponseStatus', () => {
    it('returns the status, or undefined when there is no response', () => {
        expect(getResponseStatus(makeAxiosError(401))).toBe(401);
        expect(getResponseStatus(makeAxiosError())).toBeUndefined();
        expect(getResponseStatus(new Error('boom'))).toBeUndefined();
    });
});