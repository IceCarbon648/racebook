import { createContext } from 'react';
import type { AuthContextType } from './index.types';

export const AuthContext = createContext<AuthContextType | null>(null);