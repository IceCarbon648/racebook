import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import ModCard from './index';
import { backgrounds } from '../../../assets';

const TITLE = 'Neon Skyline';
const TYPE = 'VEHICLE';
const CREATOR = 'Pat_Bonana';
const IMAGE_URL = backgrounds[0].original;

const baseProps = {
    title: TITLE,
    type: TYPE,
    imageUrl: IMAGE_URL,
    creator: CREATOR,
};

describe('ModCard', () => {
    it("shows the mod's details", () => {
        render(<ModCard {...baseProps} />);

        expect(screen.getByText(TITLE)).toBeInTheDocument();
        expect(screen.getByText(TYPE)).toBeInTheDocument();
        expect(screen.getByText(`@${CREATOR}`)).toBeInTheDocument();
        expect(screen.getByRole('img', { name: TITLE })).toHaveAttribute('src', IMAGE_URL);
    });

    it('omits the creator line when there is no creator', () => {
        render(<ModCard {...baseProps} creator={undefined} />);

        expect(screen.queryByText(`@${CREATOR}`)).not.toBeInTheDocument();
    });

    it('opens the mod when the card is clicked', async () => {
        const onClick = vi.fn();
        render(<ModCard {...baseProps} onClick={onClick} />);

        await userEvent.click(screen.getByText(TITLE));

        expect(onClick).toHaveBeenCalledOnce();
    });

    it('hides the favourite control unless both a state and a handler are given', () => {
        render(<ModCard {...baseProps} onFavourite={vi.fn()} />);
        render(<ModCard {...baseProps} isFavourite={false} />);

        expect(screen.queryByRole('button', { name: /favourites/i })).not.toBeInTheDocument();
    });

    it('labels the favourite control for the action it will perform', () => {
        const { rerender } = render(
            <ModCard {...baseProps} isFavourite={false} onFavourite={vi.fn()} />
        );

        expect(screen.getByRole('button', { name: 'Add to favourites' })).toBeInTheDocument();

        rerender(<ModCard {...baseProps} isFavourite onFavourite={vi.fn()} />);

        expect(screen.getByRole('button', { name: 'Remove from favourites' })).toBeInTheDocument();
    });

    it('favourites the mod without opening it', async () => {
        const onClick = vi.fn();
        const onFavourite = vi.fn();
        render(
            <ModCard {...baseProps} isFavourite={false} onFavourite={onFavourite} onClick={onClick} />
        );

        await userEvent.click(screen.getByRole('button', { name: 'Add to favourites' }));

        expect(onFavourite).toHaveBeenCalledOnce();
        expect(onClick).not.toHaveBeenCalled();
    });

    it('shows the owner controls only when both handlers are given', () => {
        const { rerender } = render(<ModCard {...baseProps} />);

        expect(screen.queryByRole('button', { name: 'Edit mod' })).not.toBeInTheDocument();

        rerender(<ModCard {...baseProps} onEdit={vi.fn()} />);

        expect(screen.queryByRole('button', { name: 'Edit mod' })).not.toBeInTheDocument();

        rerender(<ModCard {...baseProps} onEdit={vi.fn()} onDelete={vi.fn()} />);

        expect(screen.getByRole('button', { name: 'Edit mod' })).toBeInTheDocument();
        expect(screen.getByRole('button', { name: 'Delete mod' })).toBeInTheDocument();
    });

    it('runs the owner actions without opening the mod', async () => {
        const onClick = vi.fn();
        const onEdit = vi.fn();
        const onDelete = vi.fn();
        render(
            <ModCard {...baseProps} onClick={onClick} onEdit={onEdit} onDelete={onDelete} />
        );

        await userEvent.click(screen.getByRole('button', { name: 'Edit mod' }));
        await userEvent.click(screen.getByRole('button', { name: 'Delete mod' }));

        expect(onEdit).toHaveBeenCalledOnce();
        expect(onDelete).toHaveBeenCalledOnce();
        expect(onClick).not.toHaveBeenCalled();
    });
});