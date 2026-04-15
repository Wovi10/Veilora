import { createContext, useContext, useState } from 'react';
import type { ReactNode } from 'react';

interface EditModeContextValue {
  isEditMode: boolean;
  toggleEditMode: () => void;
}

const EditModeContext = createContext<EditModeContextValue | null>(null);

export function EditModeProvider({ children }: { children: ReactNode }) {
  const [isEditMode, setIsEditMode] = useState(() => localStorage.getItem('editMode') === 'true');

  const toggleEditMode = () => setIsEditMode(prev => {
    localStorage.setItem('editMode', String(!prev));
    return !prev;
  });

  return (
    <EditModeContext.Provider value={{ isEditMode, toggleEditMode }}>
      {children}
    </EditModeContext.Provider>
  );
}

export function useEditMode() {
  const ctx = useContext(EditModeContext);
  if (!ctx) throw new Error('useEditMode must be used within EditModeProvider');
  return ctx;
}
