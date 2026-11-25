import { describe, it, expect } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import FilePicker from './index';

const FIELD_LABEL = 'Mod File';
const ACCEPT = '.tpf';
const ID = 'modFile';

const EXISTING_NAME = 'o04a2toyr9v2weqm3tqv.tpf';
const CHOSEN_NAME = 'neon-skyline.tpf';
const NOTHING_CHOSEN = 'No file chosen';

const makeFile = (name: string) => new File(['x'], name, { type: 'application/octet-stream' });

const defaultProps: Parameters<typeof FilePicker>[0] = {
    id: ID,
    label: FIELD_LABEL,
    accept: ACCEPT,
    existingFileName: null,
};

const renderPicker = (props: Partial<typeof defaultProps> = {}) => {
    render(<FilePicker {...defaultProps} {...props} />);

    return { input: screen.getByLabelText(/choose file/i) };
};

describe('FilePicker', () => {
    it('shows the label and falls back to "No file chosen"', () => {
        renderPicker();

        expect(screen.getByText(FIELD_LABEL)).toBeInTheDocument();
        expect(screen.getByText(NOTHING_CHOSEN)).toBeInTheDocument();
    });

    it('shows the stored name, and prefers a newly chosen file over it', async () => {
        const { input } = renderPicker({ existingFileName: EXISTING_NAME });

        expect(screen.getByText(EXISTING_NAME)).toHaveAttribute('title', EXISTING_NAME);

        await userEvent.upload(input, makeFile(CHOSEN_NAME), { applyAccept: false });

        expect(screen.getByText(CHOSEN_NAME)).toBeInTheDocument();
        expect(screen.queryByText(EXISTING_NAME)).not.toBeInTheDocument();
    });

    it('exposes the input under its id so FormData can read it', () => {
        const { input } = renderPicker();

        expect(input).toHaveAttribute('id', ID);
        expect(input).toHaveAttribute('name', ID);
        expect(input).toHaveAttribute('accept', ACCEPT);
    });

    it('falls back to the stored name when the selection is cleared', async () => {
        const { input } = renderPicker({ existingFileName: EXISTING_NAME });

        await userEvent.upload(input, makeFile(CHOSEN_NAME), { applyAccept: false });
        expect(screen.getByText(CHOSEN_NAME)).toBeInTheDocument();

        fireEvent.change(input, { target: { files: [] } });

        expect(screen.getByText(EXISTING_NAME)).toBeInTheDocument();
    });
});