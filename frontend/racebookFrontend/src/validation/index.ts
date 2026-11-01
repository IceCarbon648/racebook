import * as yup from 'yup';
import {
    MAX_MOD_TITLE_LENGTH,
    MAX_MOD_DESCRIPTION_LENGTH,
} from '../constants/inputLimits';

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

const modFile = yup
    .mixed<File>()
    .test(
        'extension',
        'Mod file must be a .tpf file',
        (file) => !file || file.name.toLowerCase().endsWith('.tpf')
    )
    .test(
        'size',
        'Mod file must be smaller than 50MB',
        (file) => !file || file.size <= MAX_MOD_FILE_BYTES
    );

const previewImage = yup
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

export const createModSchema = (categories: string[], mode: 'upload' | 'edit') =>
    yup.object({
        title: yup
            .string()
            .trim()
            .required('Title is required')
            .max(MAX_MOD_TITLE_LENGTH, `Title must be ${MAX_MOD_TITLE_LENGTH} characters or fewer`),

        type: yup
            .string()
            .required('Category is required')
            .oneOf(categories, 'Select a category from the list'),

        description: yup
            .string()
            .trim()
            .required('Description is required')
            .max(
                MAX_MOD_DESCRIPTION_LENGTH,
                `Description must be ${MAX_MOD_DESCRIPTION_LENGTH} characters or fewer`
            ),

        modFile:
            mode === 'upload'
                ? modFile.required('Mod file is required')
                : modFile.nullable(),

        previewImage:
            mode === 'upload'
                ? previewImage.required('Preview image is required')
                : previewImage.nullable(),
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
    email: yup
        .string()
        .trim()
        .required('Email is required')
        .email('Enter a valid email address'),

    username: yup
        .string()
        .trim()
        .required('Username is required')
        .min(3, 'Username must be at least 3 characters')
        .max(32, 'Username must be 32 characters or fewer'),

    password: yup
        .string()
        .required('Password is required')
        .min(8, 'Password must be at least 8 characters'),

    confirmPassword: yup
        .string()
        .required('Please confirm your password')
        .oneOf([yup.ref('password')], 'Passwords do not match'),
});