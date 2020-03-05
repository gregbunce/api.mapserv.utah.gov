import React, { useState } from 'react';

export default function Endpoint(props) {
  const [collapsed, setCollapsed] = useState(true);

  return (
    <article className="bg-white shadow sm:rounded-lg ml-3 m-2 border-b border-gray-200">
      <header className="p-3 flex justify-between w-full">
        <div className="">
          <h3 className="text-2xl font-hairline tracking-tight">{props.name}</h3>
          <p className="text-sm text-gray-500 tracking-wider">{props.description}</p>
        </div>
        <div className="flex justify-end">
          <div className="flex mt-4 md:mt-0">
            <button
              onClick={() => setCollapsed(!collapsed)}
              className="flex cursor-pointer focus:outline-none items-center px-10 py-3">
              {collapsed ? (
                <svg className="h-10 w-10 fill-current text-indigo-600 hover:text-indigo-500 transition duration-150 ease-in-out" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                  <path
                    d="M9.293 12.536L5.757 9l1.415-1.414L10 10.414l2.828-2.828L14.243 9 10 13.243l-.707-.707zM20 10c0-5.523-4.477-10-10-10S0 4.477 0 10s4.477 10 10 10 10-4.477 10-10zM10 2a8 8 0 100 16 8 8 0 000-16z"
                    fill-rule="evenodd"
                  />
                </svg>
              ) : (
                <svg className="h-10 w-10 fill-current text-indigo-600 hover:text-indigo-500 transition duration-150 ease-in-out" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                  <path
                    d="M11.414 10l2.829-2.828-1.415-1.415L10 8.586 7.172 5.757 5.757 7.172 8.586 10l-2.829 2.828 1.415 1.415L10 11.414l2.828 2.829 1.415-1.415L11.414 10zM2.93 17.071c3.905 3.905 10.237 3.905 14.142 0 3.905-3.905 3.905-10.237 0-14.142-3.905-3.905-10.237-3.905-14.142 0-3.905 3.905-3.905 10.237 0 14.142zm1.414-1.414A8 8 0 1015.657 4.343 8 8 0 004.343 15.657z"
                    fill-rule="evenodd"
                  />
                </svg>
              )}</button>
          </div>
        </div>
      </header>
      {collapsed ? null : (
        <main className="bg-gray-100 border-t border-gray-200 p-8 rounded-b-lg">
          {props.children}
          <div className="flex justify-center w-full">
            <button className="flex justify-center w-3/4 self-center mt-5 px-5 py-3 border border-transparent text-base leading-6 font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-500 focus:outline-none focus:shadow-outline transition duration-150 ease-in-out">
              Send it
            </button>
          </div>
        </main>
      )}
    </article>
  );
}