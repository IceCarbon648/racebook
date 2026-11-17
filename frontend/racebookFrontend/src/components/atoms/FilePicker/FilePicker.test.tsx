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

const renderPicker = (props: Partial<Parameters<typeof FilePicker>[0]> = {}) => {
    const onChange = vi.fn();

    render(
        <FilePicker
            id={ID}
            label={FIELD_LABEL}
            accept={ACCEPT}
            file={null}
            existingFileName={null}
            onChange={onChange}
            {...props}
        />
    );

    return { onChange, input: screen.getByLabelText(/choose file/i) };
};

describe('FilePicker', () => {
    describe('what it displays', () => {
        it('shows the field label', () => {
            renderPicker();

            expect(screen.getByText(FIELD_LABEL)).toBeInTheDocument();
        });

        it('says nothing is chosen when there is no file and nothing stored', () => {
            renderPicker();

            expect(screen.getByText(NOTHING_CHOSEN)).toBeInTheDocument();
        });

        it('shows the stored file name when nothing new has been chosen', () => {
            renderPicker({ existingFileName: EXISTING_NAME });

            expect(screen.getByText(EXISTING_NAME)).toBeInTheDocument();
        });

        it('shows the chosen file name in preference to the stored one', () => {
            renderPicker({
                file: makeFile(CHOSEN_NAME),
                existingFileName: EXISTING_NAME,
            });

            expect(screen.getByText(CHOSEN_NAME)).toBeInTheDocument();
            expect(screen.queryByText(EXISTING_NAME)).not.toBeInTheDocument();
        });

        it('shows the chosen file name when there is nothing stored', () => {
            renderPicker({ file: makeFile(CHOSEN_NAME) });

            expect(screen.getByText(CHOSEN_NAME)).toBeInTheDocument();
        });

        it('repeats the displayed name as a hover title, since it may be truncated', () => {
            renderPicker({ existingFileName: EXISTING_NAME });

            expect(screen.getByText(EXISTING_NAME)).toHaveAttribute('title', EXISTING_NAME);
        });
    });

    describe('the input', () => {
        it('accepts only the given extensions', () => {
            const { input } = renderPicker();

            expect(input).toHaveAttribute('accept', ACCEPT);
        });

        it('uses the supplied id so the label points at it', () => {
            const { input } = renderPicker();

            expect(input).toHaveAttribute('id', ID);
        });
    });

    describe('choosing a file', () => {
        it('reports the chosen file', async () => {
            const { input, onChange } = renderPicker();
            const file = makeFile(CHOSEN_NAME);

            await userEvent.upload(input, file);

            expect(onChange).toHaveBeenCalledWith(file);
        });

        it('reports it only once', async () => {
            const { input, onChange } = renderPicker();

            await userEvent.upload(input, makeFile(CHOSEN_NAME));

            expect(onChange).toHaveBeenCalledOnce();
        });

        it('reports null when the selection is cleared', () => {
            const { input, onChange } = renderPicker({ existingFileName: EXISTING_NAME });

            fireEvent.change(input, { target: { files: [] } });

            expect(onChange).toHaveBeenCalledWith(null);
        });
    });
});
