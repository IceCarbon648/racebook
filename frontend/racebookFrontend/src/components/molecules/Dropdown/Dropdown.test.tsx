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
    it('shows the current value and stays collapsed until it is opened', () => {
        const { trigger } = renderDropdown();

        expect(trigger).toHaveTextContent(SELECTED);
        expect(trigger).toHaveAttribute('aria-expanded', 'false');
        expect(screen.queryByRole('listbox')).not.toBeInTheDocument();
    });

    it('reveals the options when opened, marking the current one', async () => {
        const { trigger, onChange } = renderDropdown();

        await userEvent.click(trigger);
        const [first, second] = screen.getAllByRole('option');

        expect(trigger).toHaveAttribute('aria-expanded', 'true');
        expect(screen.getAllByRole('option')).toHaveLength(OPTIONS.length);
        expect(first).toHaveAttribute('aria-selected', 'true');
        expect(second).toHaveAttribute('aria-selected', 'false');
        expect(onChange).not.toHaveBeenCalled();
    });

    it('reports the chosen value once and closes', async () => {
        const { trigger, onChange } = renderDropdown();

        await userEvent.click(trigger);
        const option = screen.getAllByRole('option')[SECOND_OPTION_INDEX];
        await userEvent.click(within(option).getByRole('button'));

        expect(onChange).toHaveBeenCalledWith(OPTIONS[SECOND_OPTION_INDEX]);
        expect(onChange).toHaveBeenCalledOnce();
        expect(screen.queryByRole('listbox')).not.toBeInTheDocument();
    });

    it('closes when the trigger is clicked again', async () => {
        const { trigger } = renderDropdown();

        await userEvent.click(trigger);
        await userEvent.click(trigger);

        expect(screen.queryByRole('listbox')).not.toBeInTheDocument();
    });

    it('closes without choosing when Escape is pressed', async () => {
        const { trigger, onChange } = renderDropdown();

        await userEvent.click(trigger);
        await userEvent.keyboard('{Escape}');

        expect(screen.queryByRole('listbox')).not.toBeInTheDocument();
        expect(onChange).not.toHaveBeenCalled();
    });

    it('closes when something outside is clicked', async () => {
        const { trigger } = renderDropdown();

        await userEvent.click(trigger);
        await userEvent.click(screen.getByRole('button', { name: 'somewhere else' }));

        expect(screen.queryByRole('listbox')).not.toBeInTheDocument();
    });

    it('opens with an empty list when there are no options', async () => {
        const { trigger } = renderDropdown({ options: [] });

        await userEvent.click(trigger);

        expect(screen.getByRole('listbox')).toBeInTheDocument();
        expect(screen.queryAllByRole('option')).toHaveLength(0);
    });
});