export interface DropdownProps {
  value: string;
  options: string[];
  label?: string;
  className?: string;
  onChange: (value: string) => void;
}
