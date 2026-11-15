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
const OVERSIZED_MOD_FILE_BYTES = 11 * ONE_KILOBYTE * ONE_KILOBYTE;
const OVERSIZED_IMAGE_BYTES = 6 * 1024 * 1024;

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

    it('requires an email', async () => {
        const errors = await validate(loginSchema, { ...validLogin, email: '' });

        expect(errors?.email).toBe('Email is required');
    });

    it('requires an email that looks like an address', async () => {
        const errors = await validate(loginSchema, { ...validLogin, email: 'racer-at-example' });

        expect(errors?.email).toBe('Enter a valid email address');
    });

    it('requires a password', async () => {
        const errors = await validate(loginSchema, { ...validLogin, password: '' });

        expect(errors?.password).toBe('Password is required');
    });

    it('reports every failing field at once', async () => {
        const errors = await validate(loginSchema, { email: '', password: '' });

        expect(errors?.email).toBeDefined();
        expect(errors?.password).toBeDefined();
    });

    it('trims surrounding whitespace before validating', async () => {
        const errors = await validate(loginSchema, {
            ...validLogin,
            email: '   racer@example.com   ',
        });

        expect(errors).toBeNull();
    });

    it('treats a whitespace only email as missing', async () => {
        const errors = await validate(loginSchema, { ...validLogin, email: '    ' });

        expect(errors?.email).toBe('Email is required');
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

    it('rejects a username below the minimum length', async () => {
        const errors = await validate(registerSchema, { ...validRegistration, Username: 'ab' });

        expect(errors?.Username).toBe('Username must be at least 3 characters');
    });

    it('rejects a username above the maximum length', async () => {
        const errors = await validate(registerSchema, {
            ...validRegistration,
            Username: 'a'.repeat(17),
        });

        expect(errors?.Username).toBe('Username must be 16 characters or fewer');
    });

    it('rejects a password below the minimum length', async () => {
        const errors = await validate(registerSchema, { ...validRegistration, Password: 'Pass1!' });

        expect(errors?.Password).toBe('Password must be at least 8 characters');
    });

    it('requires a lowercase letter in the password', async () => {
        const errors = await validate(registerSchema, {
            ...validRegistration,
            Password: 'BANANA_12',
            ConfirmPassword: 'BANANA_12',
        });

        expect(errors?.Password).toBe('Password must contain at least one lowercase letter');
    });

    it('requires an uppercase letter in the password', async () => {
        const errors = await validate(registerSchema, {
            ...validRegistration,
            Password: 'banana_12',
            ConfirmPassword: 'banana_12',
        });

        expect(errors?.Password).toBe('Password must contain at least one uppercase letter');
    });

    it('requires a number in the password', async () => {
        const errors = await validate(registerSchema, {
            ...validRegistration,
            Password: 'Banana__',
            ConfirmPassword: 'Banana__',
        });

        expect(errors?.Password).toBe('Password must contain at least one number');
    });

    it('requires a special character in the password', async () => {
        const errors = await validate(registerSchema, {
            ...validRegistration,
            Password: 'Banana12',
            ConfirmPassword: 'Banana12',
        });

        expect(errors?.Password).toBe('Password must contain at least one special character');
    });

    it('requires the confirmation to match the password', async () => {
        const errors = await validate(registerSchema, {
            ...validRegistration,
            ConfirmPassword: 'Password2!',
        });

        expect(errors?.ConfirmPassword).toBe('Passwords do not match');
    });

    it('requires a confirmation to be supplied', async () => {
        const errors = await validate(registerSchema, {
            ...validRegistration,
            ConfirmPassword: null,
        });

        expect(errors?.ConfirmPassword).toBe('Please confirm your password');
    });
});

describe('modUploadSchema', () => {
    const schema = modUploadSchema(CATEGORIES);

    it('accepts a complete upload', async () => {
        const errors = await validate(schema, validUpload);

        expect(errors).toBeNull();
    });

    it('requires a title', async () => {
        const errors = await validate(schema, { ...validUpload, title: '   ' });

        expect(errors?.title).toBe('Title is required');
    });

    it('rejects a title above the maximum length', async () => {
        const errors = await validate(schema, {
            ...validUpload,
            title: 'a'.repeat(MAX_MOD_TITLE_LENGTH + 1),
        });

        expect(errors?.title).toBe(
            `Title must be ${MAX_MOD_TITLE_LENGTH} characters or fewer`
        );
    });

    it('rejects a description above the maximum length', async () => {
        const errors = await validate(schema, {
            ...validUpload,
            description: 'a'.repeat(MAX_MOD_DESCRIPTION_LENGTH + 1),
        });

        expect(errors?.description).toBe(
            `Description must be ${MAX_MOD_DESCRIPTION_LENGTH} characters or fewer`
        );
    });

    it('rejects a category that is not in the supplied list', async () => {
        const errors = await validate(schema, { ...validUpload, type: 'SPACESHIP' });

        expect(errors?.type).toBe('Select a category from the list');
    });

    it('requires a mod file', async () => {
        const errors = await validate(schema, { ...validUpload, modFile: null });

        expect(errors?.modFile).toBe('Mod file is required');
    });

    it('requires a preview image', async () => {
        const errors = await validate(schema, { ...validUpload, previewImage: null });

        expect(errors?.previewImage).toBe('Preview image is required');
    });

    it('rejects a mod file that is not a .tpf', async () => {
        const errors = await validate(schema, {
            ...validUpload,
            modFile: makeFile('skyline.zip'),
        });

        expect(errors?.modFile).toBe('Mod file must be a .tpf file');
    });

    it('accepts a .tpf regardless of how it is capitalised', async () => {
        const errors = await validate(schema, {
            ...validUpload,
            modFile: makeFile('SKYLINE.TPF'),
        });

        expect(errors).toBeNull();
    });

    it('rejects a mod file above the size limit', async () => {
        const errors = await validate(schema, {
            ...validUpload,
            modFile: makeFile('skyline.tpf', OVERSIZED_MOD_FILE_BYTES),
        });

        expect(errors?.modFile).toBe('Mod file must be smaller than 10MB');
    });

    it('rejects a preview image that is not png, jpg or jpeg', async () => {
        const errors = await validate(schema, {
            ...validUpload,
            previewImage: makeFile('preview.gif'),
        });

        expect(errors?.previewImage).toBe('Preview image must be a .png, .jpg or .jpeg file');
    });

    it('accepts a jpeg preview image', async () => {
        const errors = await validate(schema, {
            ...validUpload,
            previewImage: makeFile('preview.jpeg'),
        });

        expect(errors).toBeNull();
    });

    it('rejects a preview image above the size limit', async () => {
        const errors = await validate(schema, {
            ...validUpload,
            previewImage: makeFile('preview.png', OVERSIZED_IMAGE_BYTES),
        });

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

    it('still rejects a replacement file of the wrong type', async () => {
        const errors = await validate(schema, {
            ...validModDetails,
            modFile: makeFile('skyline.exe'),
            previewImage: null,
        });

        expect(errors?.modFile).toBe('Mod file must be a .tpf file');
    });

    it('still rejects a replacement file above the size limit', async () => {
        const errors = await validate(schema, {
            ...validModDetails,
            modFile: null,
            previewImage: makeFile('preview.png', OVERSIZED_IMAGE_BYTES),
        });

        expect(errors?.previewImage).toBe('Preview image must be smaller than 5MB');
    });
});