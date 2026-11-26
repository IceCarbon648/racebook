import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, within, fireEvent } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import ModModal from '../../../components/organisms/ModModal';
import type { MyMod } from '../../../types';
import { createWrapper } from '../../../test/helpers/queryWrapper';

const { useCategoriesMock, notifyMock } = vi.hoisted(() => ({
    useCategoriesMock: vi.fn(),
    notifyMock: {
        success: vi.fn(),
        error: vi.fn(),
        apiError: vi.fn(),
    },
}));

vi.mock('../../../hooks/api/useCategories', () => ({
    useCategories: useCategoriesMock,
}));

vi.mock('../../../utils/notify', () => ({
    notify: notifyMock,
}));

const CATEGORIES = ['ENVIRONMENT', 'VEHICLE', 'PACK'];
const CHOSEN_CATEGORY = CATEGORIES[0];

const TITLE = 'Neon Skyline';
const DESCRIPTION = 'A repaint of the R34 with neon underglow.';

const EXISTING_MOD = {
    modId: '22222222-2222-2222-2222-222222222222',
    title: 'Midnight Expressway',
    type: 'ENVIRONMENT',
    description: 'A resurfaced version of the motorway stage.',
    modFileUrl: 'https://res.cloudinary.com/racebook/raw/upload/v1/Mods/o04a2toyr9v2weqm3tqv.tpf',
    imageUrl: 'https://res.cloudinary.com/racebook/image/upload/v1/PreviewImages/sbhal99jwoe39i9sejoz.png',
} as unknown as MyMod;

const SERVER_REJECTION = Object.assign(new Error('Request failed'), {
    isAxiosError: true,
    response: { data: { message: 'Upload service is unavailable' }, status: 502 },
});

const renderModal = (props: Partial<Parameters<typeof ModModal>[0]> = {}) => {
    const onClose = vi.fn();
    const onSubmit = vi.fn().mockResolvedValue(undefined);
    const { wrapper } = createWrapper();

    const view = render(
        <ModModal mode="upload" onClose={onClose} onSubmit={onSubmit} {...props} />,
        { wrapper }
    );

    return { onClose, onSubmit, ...view };
};

const renderEditModal = () => renderModal({ mode: 'edit', mod: EXISTING_MOD });

const fillTextFields = async () => {
    const title = screen.getByLabelText('Title');
    await userEvent.clear(title);
    await userEvent.type(title, TITLE);

    const description = screen.getByLabelText('Description');
    await userEvent.clear(description);
    await userEvent.type(description, DESCRIPTION);

    await userEvent.click(screen.getByRole('button', { name: 'Select a category' }));
    const option = screen.getAllByRole('option')[CATEGORIES.indexOf(CHOSEN_CATEGORY)];
    await userEvent.click(within(option).getByRole('button'));
};

const submit = () =>
    userEvent.click(screen.getByRole('button', { name: /upload|save changes/i }));

beforeEach(() => {
    vi.clearAllMocks();
    useCategoriesMock.mockReturnValue({
        data: CATEGORIES,
        isLoading: false,
        isError: false,
    });
});

describe('ModModal', () => {
    it('renders nothing when it is closed', () => {

        expect(screen.queryByRole('heading')).not.toBeInTheDocument();
    });

    it('pre-fills the form from the mod being edited', () => {
        renderModal({ mode: 'edit', mod: EXISTING_MOD });

        expect(screen.getByRole('heading', { name: 'Edit Mod' })).toBeInTheDocument();
        expect(screen.getByLabelText('Title')).toHaveValue(EXISTING_MOD.title);
        expect(screen.getByLabelText('Description')).toHaveValue(EXISTING_MOD.description);
        expect(screen.getByText('o04a2toyr9v2weqm3tqv.tpf')).toBeInTheDocument();
        expect(screen.getByText('sbhal99jwoe39i9sejoz.png')).toBeInTheDocument();
    });

    it('explains the problem and blocks submission when the categories fail to load', () => {
        useCategoriesMock.mockReturnValue({
            data: [],
            isLoading: false,
            isError: true,
        });

        renderModal();

        expect(screen.getByText(/could not load categories/i)).toBeInTheDocument();
        expect(screen.getByRole('button', { name: 'UPLOAD' })).toBeDisabled();
    });

    it('refuses an empty form and says what is missing', async () => {
        const { onSubmit } = renderModal();

        await submit();

        expect(onSubmit).not.toHaveBeenCalled();
        expect(await screen.findByText('Title is required')).toBeInTheDocument();
        expect(screen.getByText('Select a category from the list')).toBeInTheDocument();
        expect(screen.getByText('Description is required')).toBeInTheDocument();
        expect(screen.getByText('Mod file is required')).toBeInTheDocument();
        expect(screen.getByText('Preview image is required')).toBeInTheDocument();
    });

    it('allows an edit that changes no files', async () => {
        const { onSubmit } = renderModal({ mode: 'edit', mod: EXISTING_MOD });

        await submit();

        expect(onSubmit).toHaveBeenCalledOnce();
    });

    it('sends what was entered', async () => {
        const { onSubmit } = renderEditModal();

        await fillTextFields();
        await submit();

        const formData = onSubmit.mock.calls[0][0] as FormData;

        expect(formData.get('title')).toBe(TITLE);
        expect(formData.get('description')).toBe(DESCRIPTION);
        expect(formData.get('type')).toBe(CHOSEN_CATEGORY);
    });

    it('confirms and closes once the edit succeeds', async () => {
        const { onClose } = renderEditModal();

        await fillTextFields();
        await submit();

        expect(notifyMock.success).toHaveBeenCalledWith('Changes saved');
        expect(onClose).toHaveBeenCalledOnce();
    });

    it('shows the server’s reason and stays open when the save fails', async () => {
        const { onSubmit, onClose } = renderEditModal();
        onSubmit.mockRejectedValue(SERVER_REJECTION);

        await fillTextFields();
        await submit();

        expect(await screen.findByText('Upload service is unavailable')).toBeInTheDocument();
        expect(onClose).not.toHaveBeenCalled();
        expect(notifyMock.success).not.toHaveBeenCalled();
    });

    it('closes from the backdrop but not from the form itself', async () => {
        const { onClose, container } = renderModal();

        await userEvent.click(screen.getByLabelText('Title'));
        expect(onClose).not.toHaveBeenCalled();

        fireEvent.click(container.firstChild as HTMLElement);
        expect(onClose).toHaveBeenCalledOnce();
    });
});
