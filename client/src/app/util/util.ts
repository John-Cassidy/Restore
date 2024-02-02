export const getCookie = (key: string) => {
  const b = document.cookie.match('(^|;)\\s*' + key + '\\s*=\\s*([^;]+)');
  return b ? b.pop() : '';
};
export const currencyFormat = (amount: number) => {
  return '$' + (amount / 100).toFixed(2);
};

export const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString();
};
