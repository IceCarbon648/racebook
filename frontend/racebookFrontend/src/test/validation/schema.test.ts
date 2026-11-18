import { describe, it, expect } from 'vitest';
import {
    validate,
    loginSchema,
    registerSchema,
    modUploadSchema,
    modEditSchema,
} from '../../validation';
import {
    MAX_MOD_TITLE_LENGTH,
    MAX_MOD_DESCRIPTION_LENGTH,
} from '../../constants/inputLimits';

const CATEGORIES = ['VEHICLE', 'ENVIRONMENT', 'UI'];

const ONE_KILOBYTE = 1024;
const ONE_MEGABYTE = ONE_KILOBYTE * ONE_KILOBYTE;
const OVERSIZED_MOD_FILE_BYTES = 11 * ONE_MEGABYTE;
const OVERSIZED_IMAGE_BYTES = 6 * ONE_MEGABYTE;

const makeFile = (name: string, sizeInBytes: number = ONE_KILOBYTE) => {
    const file = new File(['x'], name);
    Object.defineProperty(file, 'size', { value: sizeInBytes });
    return file;
};

const validLogin = {
    email: 'racer@example.com',
    password: 'Banana_12',
};

const validRegistration = {
    Email: 'racer@example.com',
    Username: 'SpeedDemon',
    Password: 'Banana_12',
    ConfirmPassword: 'Banana_12',
};

const validModDetails = {
    title: 'Neon Skyline',
    type: 'VEHICLE',
    description: 'A repaint of the R34 with neon underglow.',
};

const validUpload = {
    ...validModDetails,
    modFile: makeFile('skyline.tpf'),
    previewImage: makeFile('preview.png'),
};

describe('loginSchema', () => {
    it('accepts a valid email and password', async () => {
        const errors = await validate(loginSchema, validLogin);

        expect(errors).toBeNull();
    });

    it('requires both fields', async () => {
        const errors = await validate(loginSchema, { email: '', password: '' });

        expect(errors?.email).toBe('Email is required');
        expect(errors?.password).toBe('Password is required');
    });

    it('requires an email that looks like an address', async () => {
        const errors = await validate(loginSchema, { ...validLogin, email: 'racer-at-example' });

        expect(errors?.email).toBe('Enter a valid email address');
    });

    it('trims the email before validating it', async () => {
        const padded = await validate(loginSchema, {
            ...validLogin,
            email: '   racer@example.com   ',
        });

        const blank = await validate(loginSchema, { ...validLogin, email: '    ' });

        expect(padded).toBeNull();
        expect(blank?.email).toBe('Email is required');
    });
});

describe('registerSchema', () => {
    it('accepts a valid registration', async () => {
        const errors = await validate(registerSchema, validRegistration);

        expect(errors).toBeNull();
    });

    it('keys its errors to the API field names', async () => {
        const errors = await validate(registerSchema, {
            Email: '',
            Username: '',
            Password: '',
            ConfirmPassword: '',
        });

        expect(Object.keys(errors ?? {})).toEqual(
            expect.arrayContaining(['Email', 'Username', 'Password', 'ConfirmPassword'])
        );
        expect(errors).not.toHaveProperty('email');
    });

    it('rejects a username outside the allowed length', async () => {
        const tooShort = await validate(registerSchema, { ...validRegistration, Username: 'ab' });

        const tooLong = await validate(registerSchema, {
            ...validRegistration,
            Username: 'a'.repeat(17),
        });

        expect(tooShort?.Username).toBe('Username must be at least 3 characters');
        expect(tooLong?.Username).toBe('Username must be 16 characters or fewer');
    });

    it('rejects a password below the minimum length', async () => {
        const errors = await validate(registerSchema, {
            ...validRegistration,
            Password: 'Pass1!',
            ConfirmPassword: 'Pass1!',
        });

        expect(errors?.Password).toBe('Password must be at least 8 characters');
    });

    it('requires a mix of character types in the password', async () => {
        const withoutLowercase = await validate(registerSchema, {
            ...validRegistration,
            Password: 'BANANA_12',
            ConfirmPassword: 'BANANA_12',
        });

        const withoutUppercase = await validate(registerSchema, {
            ...validRegistration,
            Password: 'banana_12',
            ConfirmPassword: 'banana_12',
        });

        const withoutNumber = await validate(registerSchema, {
            ...validRegistration,
            Password: 'Banana__',
            ConfirmPassword: 'Banana__',
        });

        const withoutSymbol = await validate(registerSchema, {
            ...validRegistration,
            Password: 'Banana12',
            ConfirmPassword: 'Banana12',
        });

        expect(withoutLowercase?.Password).toBe('Password must contain at least one lowercase letter');
        expect(withoutUppercase?.Password).toBe('Password must contain at least one uppercase letter');
        expect(withoutNumber?.Password).toBe('Password must contain at least one number');
        expect(withoutSymbol?.Password).toBe('Password must contain at least one special character');
    });

    it('requires a confirmation that matches the password', async () => {
        const mismatched = await validate(registerSchema, {
            ...validRegistration,
            ConfirmPassword: 'Password2!',
        });

        const missing = await validate(registerSchema, {
            ...validRegistration,
            ConfirmPassword: null,
        });

        expect(mismatched?.ConfirmPassword).toBe('Passwords do not match');
        expect(missing?.ConfirmPassword).toBe('Please confirm your password');
    });
});

describe('modUploadSchema', () => {
    const schema = modUploadSchema(CATEGORIES);

    it('accepts a complete upload', async () => {
        const errors = await validate(schema, validUpload);

        expect(errors).toBeNull();
    });

    it('requires the text fields', async () => {
        const errors = await validate(schema, { ...validUpload, title: '   ', description: '' });

        expect(errors?.title).toBe('Title is required');
        expect(errors?.description).toBe('Description is required');
    });

    it('rejects text above the maximum length', async () => {
        const errors = await validate(schema, {
            ...validUpload,
            title: 'a'.repeat(MAX_MOD_TITLE_LENGTH + 1),
            description: 'a'.repeat(MAX_MOD_DESCRIPTION_LENGTH + 1),
        });

        expect(errors?.title).toBe(`Title must be ${MAX_MOD_TITLE_LENGTH} characters or fewer`);
        expect(errors?.description).toBe(
            `Description must be ${MAX_MOD_DESCRIPTION_LENGTH} characters or fewer`
        );
    });

    it('rejects a category that is not in the supplied list', async () => {
        const errors = await validate(schema, { ...validUpload, type: 'SPACESHIP' });

        expect(errors?.type).toBe('Select a category from the list');
    });

    it('requires both files', async () => {
        const errors = await validate(schema, {
            ...validUpload,
            modFile: null,
            previewImage: null,
        });

        expect(errors?.modFile).toBe('Mod file is required');
        expect(errors?.previewImage).toBe('Preview image is required');
    });

    it('rejects files with the wrong extension', async () => {
        const errors = await validate(schema, {
            ...validUpload,
            modFile: makeFile('skyline.zip'),
            previewImage: makeFile('preview.gif'),
        });

        expect(errors?.modFile).toBe('Mod file must be a .tpf file');
        expect(errors?.previewImage).toBe('Preview image must be a .png, .jpg or .jpeg file');
    });

    it('accepts the allowed extensions whatever the case', async () => {
        const errors = await validate(schema, {
            ...validUpload,
            modFile: makeFile('SKYLINE.TPF'),
            previewImage: makeFile('PREVIEW.JPEG'),
        });

        expect(errors).toBeNull();
    });

    it('rejects files above the size limit', async () => {
        const errors = await validate(schema, {
            ...validUpload,
            modFile: makeFile('skyline.tpf', OVERSIZED_MOD_FILE_BYTES),
            previewImage: makeFile('preview.png', OVERSIZED_IMAGE_BYTES),
        });

        expect(errors?.modFile).toBe('Mod file must be smaller than 10MB');
        expect(errors?.previewImage).toBe('Preview image must be smaller than 5MB');
    });
});

describe('modEditSchema', () => {
    const schema = modEditSchema(CATEGORIES);

    it('accepts an edit with no files at all', async () => {
        const errors = await validate(schema, {
            ...validModDetails,
            modFile: null,
            previewImage: null,
        });

        expect(errors).toBeNull();
    });

    it('still requires the text fields', async () => {
        const errors = await validate(schema, {
            title: '',
            type: '',
            description: '',
            modFile: null,
            previewImage: null,
        });

        expect(errors?.title).toBe('Title is required');
        expect(errors?.type).toBe('Select a category from the list');
        expect(errors?.description).toBe('Description is required');
    });

    it('still applies the file rules when a file is supplied', async () => {
        const errors = await validate(schema, {
            ...validModDetails,
            modFile: makeFile('skyline.exe'),
            previewImage: makeFile('preview.png', OVERSIZED_IMAGE_BYTES),
        });

        expect(errors?.modFile).toBe('Mod file must be a .tpf file');
        expect(errors?.previewImage).toBe('Preview image must be smaller than 5MB');
    });
});