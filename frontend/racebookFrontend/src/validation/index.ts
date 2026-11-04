import * as yup from 'yup';
import { MAX_MOD_TITLE_LENGTH, MAX_MOD_DESCRIPTION_LENGTH, } from '../constants/inputLimits';

const MAX_MOD_FILE_BYTES = 10 * 1024 * 1024;
const MAX_PREVIEW_IMAGE_BYTES = 5 * 1024 * 1024;

export const validate = async <T extends object>(
    schema: yup.AnyObjectSchema,
    values: T
): Promise<Record<string, string> | null> => {
    try {
        await schema.validate(values, { abortEarly: false });
        return null;
    } catch (err) {
        if (err instanceof yup.ValidationError) {
            const errors: Record<string, string> = {};

            err.inner.forEach((issue) => {
                if (issue.path && !errors[issue.path]) {
                    errors[issue.path] = issue.message;
                }
            });

            return errors;
        }

        throw err;
    }
};

const modFileRules = yup
    .mixed<File>()
    .test(
        'extension',
        'Mod file must be a .tpf file',
        (file) => !file || file.name.toLowerCase().endsWith('.tpf')
    )
    .test(
        'size',
        'Mod file must be smaller than 10MB',
        (file) => !file || file.size <= MAX_MOD_FILE_BYTES
    );

const previewImageRules = yup
    .mixed<File>()
    .test(
        'extension',
        'Preview image must be a .png, .jpg or .jpeg file',
        (file) => !file || /\.(png|jpe?g)$/i.test(file.name)
    )
    .test(
        'size',
        'Preview image must be smaller than 5MB',
        (file) => !file || file.size <= MAX_PREVIEW_IMAGE_BYTES
    );

const titleRules = yup
    .string()
    .trim()
    .max(MAX_MOD_TITLE_LENGTH, `Title must be ${MAX_MOD_TITLE_LENGTH} characters or fewer`);

const descriptionRules = yup
    .string()
    .trim()
    .max(
        MAX_MOD_DESCRIPTION_LENGTH,
        `Description must be ${MAX_MOD_DESCRIPTION_LENGTH} characters or fewer`
    );

const typeRules = (categories: string[]) =>
    yup.string().oneOf(categories, 'Select a category from the list');

const modDetailsSchema = (categories: string[]) =>
    yup.object({
        title: titleRules.required('Title is required'),
        type: typeRules(categories).required('Category is required'),
        description: descriptionRules.required('Description is required'),
    });

export const modUploadSchema = (categories: string[]) =>
    modDetailsSchema(categories).shape({
        modFile: modFileRules.required('Mod file is required'),
        previewImage: previewImageRules.required('Preview image is required'),
    });

export const modEditSchema = (categories: string[]) =>
    modDetailsSchema(categories).shape({
        modFile: modFileRules.nullable(),
        previewImage: previewImageRules.nullable(),
    });

export const loginSchema = yup.object({
    email: yup
        .string()
        .trim()
        .required('Email is required')
        .email('Enter a valid email address'),

    password: yup.string().required('Password is required'),
});

export const registerSchema = yup.object({
    Email: yup
        .string()
        .trim()
        .required('Email is required')
        .email('Enter a valid email address'),

    Username: yup
        .string()
        .trim()
        .required('Username is required')
        .min(3, 'Username must be at least 3 characters')
        .max(16, 'Username must be 16 characters or fewer'),

    Password: yup
        .string()
        .required('Password is required')
        .min(8, 'Password must be at least 8 characters')
        .matches(/[a-z]/, 'Password must contain at least one lowercase letter')
        .matches(/[A-Z]/, 'Password must contain at least one uppercase letter')
        .matches(/[0-9]/, 'Password must contain at least one number')
        .matches(/[^a-zA-Z0-9]/, 'Password must contain at least one special character'),

    ConfirmPassword: yup
        .string()
        .required('Please confirm your password')
        .oneOf([yup.ref('Password')], 'Passwords do not match'),
});