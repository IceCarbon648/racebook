import { describe, it, expect } from 'vitest';
import { AxiosError, type AxiosResponse } from 'axios';
import { getResponseMessage, getResponseStatus } from '../../utils/getResponseMessage';

const DEFAULT_FALLBACK = 'Something went wrong, please try again';

/**
 * Builds a real AxiosError so that axios.isAxiosError recognises it.
 * Omit the status to simulate a request that never reached the server.
 */
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
    });

    it('falls back when the value is not an error at all', () => {
        expect(getResponseMessage('boom')).toBe(DEFAULT_FALLBACK);
    });

    it('uses the supplied fallback instead of the default', () => {
        expect(getResponseMessage(new Error('boom'), 'Could not delete the mod')).toBe(
            'Could not delete the mod'
        );
    });

    it('falls back when the request never reached the server', () => {
        expect(getResponseMessage(makeAxiosError())).toBe(DEFAULT_FALLBACK);
    });

    it('joins field errors from a validation response', () => {
        const error = makeAxiosError(400, {
            errors: { Email: ['Email is required'] },
        });

        expect(getResponseMessage(error)).toBe('Email is required');
    });

    it('joins field errors across several fields and messages', () => {
        const error = makeAxiosError(400, {
            errors: {
                Email: ['Email is required'],
                Password: ['Too short', 'Needs a number'],
            },
        });

        expect(getResponseMessage(error)).toBe('Email is required Too short Needs a number');
    });

    it('prefers field errors over a top level message', () => {
        const error = makeAxiosError(400, {
            errors: { Email: ['Email is required'] },
            message: 'Validation failed',
        });

        expect(getResponseMessage(error)).toBe('Email is required');
    });

    it('uses the message when there are no field errors', () => {
        const error = makeAxiosError(409, { message: 'Registration failed' });

        expect(getResponseMessage(error)).toBe('Registration failed');
    });

    it('prefers the message over the detail', () => {
        const error = makeAxiosError(500, {
            message: 'Registration failed',
            detail: 'An unexpected error occurred',
        });

        expect(getResponseMessage(error)).toBe('Registration failed');
    });

    it('falls back to the detail from a ProblemDetails response', () => {
        const error = makeAxiosError(500, {
            title: 'An error occurred',
            detail: 'An unexpected error occurred',
        });

        expect(getResponseMessage(error)).toBe('An unexpected error occurred');
    });

    it('falls back when the response body carries nothing usable', () => {
        expect(getResponseMessage(makeAxiosError(500, {}))).toBe(DEFAULT_FALLBACK);
    });

    it('falls back when the response body is not an object', () => {
        expect(getResponseMessage(makeAxiosError(502, 'Bad Gateway'))).toBe(DEFAULT_FALLBACK);
    });

    it('falls back when the errors object is present but empty', () => {
        const error = makeAxiosError(400, { errors: {}, message: 'Validation failed' });

        expect(getResponseMessage(error)).toBe('Validation failed');
    });
});

describe('getResponseStatus', () => {
    it('returns the status from an axios error', () => {
        expect(getResponseStatus(makeAxiosError(401))).toBe(401);
    });

    it('returns undefined when the request never reached the server', () => {
        expect(getResponseStatus(makeAxiosError())).toBeUndefined();
    });

    it('returns undefined when the error did not come from axios', () => {
        expect(getResponseStatus(new Error('boom'))).toBeUndefined();
    });
});