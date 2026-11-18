import { describe, it, expect, vi } from 'vitest';
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
    file: null,
    existingFileName: null,
    onChange: () => {},
};

const renderPicker = (props: Partial<typeof defaultProps> = {}) => {
    const onChange = vi.fn();

    render(<FilePicker {...defaultProps} onChange={onChange} {...props} />);

    return { onChange, input: screen.getByLabelText(/choose file/i) };
};

describe('FilePicker', () => {
    it('shows the stored name, and prefers a newly chosen file over it', () => {
        const { rerender } = render(<FilePicker {...defaultProps} />);

        expect(screen.getByText(FIELD_LABEL)).toBeInTheDocument();
        expect(screen.getByText(NOTHING_CHOSEN)).toBeInTheDocument();

        rerender(<FilePicker {...defaultProps} existingFileName={EXISTING_NAME} />);

        expect(screen.getByText(EXISTING_NAME)).toHaveAttribute('title', EXISTING_NAME);

        rerender(
            <FilePicker
                {...defaultProps}
                existingFileName={EXISTING_NAME}
                file={makeFile(CHOSEN_NAME)}
            />
        );

        expect(screen.getByText(CHOSEN_NAME)).toBeInTheDocument();
        expect(screen.queryByText(EXISTING_NAME)).not.toBeInTheDocument();
    });

    it('passes its id and accept through to the input', () => {
        const { input } = renderPicker();

        expect(input).toHaveAttribute('id', ID);
        expect(input).toHaveAttribute('accept', ACCEPT);
    });

    it('reports the chosen file', async () => {
        const { input, onChange } = renderPicker();
        const file = makeFile(CHOSEN_NAME);

        await userEvent.upload(input, file);

        expect(onChange).toHaveBeenCalledWith(file);
        expect(onChange).toHaveBeenCalledOnce();
    });

    it('reports null when the selection is cleared', () => {
        const { input, onChange } = renderPicker({ existingFileName: EXISTING_NAME });

        fireEvent.change(input, { target: { files: [] } });

        expect(onChange).toHaveBeenCalledWith(null);
    });
});