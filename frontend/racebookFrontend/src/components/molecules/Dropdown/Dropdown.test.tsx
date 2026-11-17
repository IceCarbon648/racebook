import { describe, it, expect, vi } from 'vitest';
import { render, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import Dropdown from './index';

const OPTIONS = ['ENVIRONMENT', 'VEHICLE', 'PACK'];
const LABEL = 'Filter by category';

const SELECTED = OPTIONS[0];
const SECOND_OPTION_INDEX = 1;

const renderDropdown = (props: Partial<Parameters<typeof Dropdown>[0]> = {}) => {
    const onChange = vi.fn();

    render(
        <div>
            <Dropdown
                value={SELECTED}
                options={OPTIONS}
                label={LABEL}
                onChange={onChange}
                {...props}
            />
            <button type="button">somewhere else</button>
        </div>
    );

    return { onChange, trigger: screen.getByRole('button', { name: LABEL }) };
};

describe('Dropdown', () => {
    describe('when closed', () => {
        it('shows the current value on the trigger', () => {
            const { trigger } = renderDropdown();

            expect(trigger).toHaveTextContent(SELECTED);
        });

        it('takes its accessible name from the label', () => {
            renderDropdown();

            expect(screen.getByRole('button', { name: LABEL })).toBeInTheDocument();
        });

        it('reports that it is collapsed', () => {
            const { trigger } = renderDropdown();

            expect(trigger).toHaveAttribute('aria-expanded', 'false');
        });

        it('renders no list', () => {
            renderDropdown();

            expect(screen.queryByRole('listbox')).not.toBeInTheDocument();
        });
    });

    describe('when opened', () => {
        it('reveals the list', async () => {
            const { trigger } = renderDropdown();

            await userEvent.click(trigger);

            expect(screen.getByRole('listbox')).toBeInTheDocument();
        });

        it('reports that it is expanded', async () => {
            const { trigger } = renderDropdown();

            await userEvent.click(trigger);

            expect(trigger).toHaveAttribute('aria-expanded', 'true');
        });

        it('renders one option per choice', async () => {
            const { trigger } = renderDropdown();

            await userEvent.click(trigger);

            expect(screen.getAllByRole('option')).toHaveLength(OPTIONS.length);
        });

        it('marks the current value as selected', async () => {
            const { trigger } = renderDropdown();

            await userEvent.click(trigger);
            const [first, second] = screen.getAllByRole('option');

            expect(first).toHaveAttribute('aria-selected', 'true');
            expect(second).toHaveAttribute('aria-selected', 'false');
        });

        it('does not report a choice just for opening', async () => {
            const { trigger, onChange } = renderDropdown();

            await userEvent.click(trigger);

            expect(onChange).not.toHaveBeenCalled();
        });
    });

    describe('choosing an option', () => {
        const openAndChoose = async (index: number) => {
            const { trigger, onChange } = renderDropdown();

            await userEvent.click(trigger);

            const option = screen.getAllByRole('option')[index];
            await userEvent.click(within(option).getByRole('button'));

            return { onChange, trigger };
        };

        it('reports the chosen value', async () => {
            const { onChange } = await openAndChoose(SECOND_OPTION_INDEX);

            expect(onChange).toHaveBeenCalledWith(OPTIONS[SECOND_OPTION_INDEX]);
        });

        it('reports it only once', async () => {
            const { onChange } = await openAndChoose(SECOND_OPTION_INDEX);

            expect(onChange).toHaveBeenCalledOnce();
        });

        it('closes the list', async () => {
            await openAndChoose(SECOND_OPTION_INDEX);

            expect(screen.queryByRole('listbox')).not.toBeInTheDocument();
        });
    });

    describe('dismissing without choosing', () => {
        it('closes when the trigger is clicked again', async () => {
            const { trigger } = renderDropdown();

            await userEvent.click(trigger);
            await userEvent.click(trigger);

            expect(screen.queryByRole('listbox')).not.toBeInTheDocument();
        });

        it('closes when Escape is pressed', async () => {
            const { trigger } = renderDropdown();

            await userEvent.click(trigger);
            await userEvent.keyboard('{Escape}');

            expect(screen.queryByRole('listbox')).not.toBeInTheDocument();
        });

        it('closes when something outside is clicked', async () => {
            const { trigger } = renderDropdown();

            await userEvent.click(trigger);
            await userEvent.click(screen.getByRole('button', { name: 'somewhere else' }));

            expect(screen.queryByRole('listbox')).not.toBeInTheDocument();
        });

        it('reports no choice when dismissed', async () => {
            const { trigger, onChange } = renderDropdown();

            await userEvent.click(trigger);
            await userEvent.keyboard('{Escape}');

            expect(onChange).not.toHaveBeenCalled();
        });
    });

    describe('with no options', () => {
        it('still opens, showing an empty list', async () => {
            const { trigger } = renderDropdown({ options: [] });

            await userEvent.click(trigger);

            expect(screen.getByRole('listbox')).toBeInTheDocument();
            expect(screen.queryAllByRole('option')).toHaveLength(0);
        });
    });
});
